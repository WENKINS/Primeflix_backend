using Primeflix.Services.Authentication;
using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.ProductService;
using Stripe;
using Stripe.Checkout;

namespace Primeflix.Services.PaymentService
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ICartRepository _cartRepository;
        private readonly IAuthentication _authentication;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        const string secret = "whsec_0e5c563329e8f57680b5144f4ae511c1836442cbfd94fe2b53ddbffab70306a8";

        public PaymentRepository(ICartRepository cartRepository, IAuthentication authentication, IOrderRepository orderRepository, IProductRepository productRepository)
        {
            StripeConfiguration.ApiKey = "sk_test_51MLTT4DVJ3BzSGwXY3qcyRDPEDYKVja1xDRadx3LlnZE4tR5ebagsQzfLtauhnBYJ0WuYVudXsXr4GUap757Ryes00xV5s7Lxe";
            _cartRepository = cartRepository;
            _authentication = authentication;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Session> CreateCheckoutSession(int cartId)
        {
            var cartItems = await _cartRepository.GetProductsOfACart(cartId);
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var cartItem in cartItems)
            {
                var product = await _productRepository.GetProduct(cartItem.ProductId);
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (decimal)product.Price * 100,
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Title,
                            Images = new List<string> { product.PictureUrl }
                        }

                    },
                    Quantity = cartItem.Quantity
                });

            }

            var cart = await _cartRepository.GetCart(cartId);
            var user = await _authentication.GetUser(cart.UserId);

            var options = new SessionCreateOptions
            {
                CustomerEmail = user.Email,
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:5000/order-success",
                CancelUrl = "https://localhost:5000/order-success"
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return session;
        }

        public async Task<bool> FulfillOrder(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try 
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    secret
                    );
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authentication.GetUser(session.CustomerEmail);
                    var cart = await _cartRepository.GetCartOfAUser(user.Id);
                    await _orderRepository.PlaceOrder(cart.Id);
                }

                return true;
            } 
            catch (StripeException e)
            {
                return false;
            }
        }
    }
}
