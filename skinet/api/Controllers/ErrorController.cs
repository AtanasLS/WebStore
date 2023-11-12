using api.Errors;
using Microsoft.AspNetCore.Mvc;
namespace api.Controllers
{
    [Route("/errors/{code}")]
    public class ErrorController : BaseApiController
    {
        public IActionResult Error (int code)
        {
            return new ObjectResult(new ApiResponse(code)); 
        }
    }
}