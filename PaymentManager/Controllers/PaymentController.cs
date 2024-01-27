using Microsoft.AspNetCore.Mvc;

namespace PaymentManager.Controllers
{
    [ApiController]
    [Route("api/[controller]/payment")]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/GetPaymentControllerName")]
        public string GetPaymentControllerName()
        {
            return "Payment Controller";
        }
    }
}
