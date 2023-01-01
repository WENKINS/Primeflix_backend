using Stripe.Checkout;

namespace Primeflix.Services.PaymentService
{
    public interface IPaymentRepository
    {
        public Task<Session> CreateCheckoutSession(int cartId);
        public Task<bool> FulfillOrder(HttpRequest request);
    }
}
