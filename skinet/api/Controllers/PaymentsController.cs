using System.Drawing.Printing;
using api.Errors;
using Core.Entities;
using Core.Entities.OrederAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace api.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private const string WhSecret = "whsec_d5144e96d9926ca879aae8e6513feb528a83d8189867abc91c94e018c4018356";
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> logger;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            this.logger = logger;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentUpdate(string basketId)
        {
            var basket =  await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if(basket == null) return BadRequest(new ApiResponse(400, "Problem With your basket!"));

            return basket;
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);
            
            PaymentIntent intent; 
            Order order; 

            switch(stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    logger.LogInformation("Payment succeeded: " + intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    logger.LogInformation("Order updated to payment recieved: " + order.Id);

                break;
                 case "payment_intent.payment_failed":
                    intent = (PaymentIntent) stripeEvent.Data.Object;
                    logger.LogInformation("Payment failed: " + intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    logger.LogInformation("Order updated to payment recieved: " + order.Id);
                break;
            }

            return new EmptyResult();
        }
        

    }
}