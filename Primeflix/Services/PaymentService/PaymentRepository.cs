using Primeflix.Services.CartService;
using Primeflix.Services.OrderService;
using Primeflix.Services.ProductService;
using Primeflix.Services.UserService;
using Stripe;
using Stripe.Checkout;

namespace Primeflix.Services.PaymentService
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IConfiguration _configuration;

        public PaymentRepository(
            ICartRepository cartRepository, 
            IUserRepository userRepository, 
            IOrderRepository orderRepository, 
            IProductRepository productRepository,
            IConfiguration configuration
            )
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe:Key").Value;
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
            var user = await _userRepository.GetUser(cart.UserId);

            var options = new SessionCreateOptions
            {
                CustomerEmail = user.Email,
                ShippingAddressCollection =
                    new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> { "BE" }
                    },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:8080/order-success",
                CancelUrl = "https://localhost:8080/order-success"
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
                    _configuration.GetSection("Stripe:Secret").Value
                    );
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _userRepository.GetUser(session.CustomerEmail);
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
