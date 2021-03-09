using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SparvPipelineProxy.Utils;

namespace SparvPipelineProxy.Controllers
{
    [ApiController]
    public class SparvPipelineController : ControllerBase
    {
        [HttpPost("/annotate/sparv")]
        public async Task<IActionResult> Annotate(AnnotationRequest request)
        {
            if (request.LanguageCode != "swe")
            {
                return Problem(statusCode: (int)HttpStatusCode.BadRequest, title: "Only Swedish (swe) is supported for now.");
            }

            return Content(
                await CommandService.AnnotateWithSparvPipeline(request.LanguageCode, request.Input),
                "text/xml",
                Encoding.UTF8
            );
        }

        [HttpGet("/healthcheck")]
        public async Task<IActionResult> HealthCheck()
        {
            return Ok();
        }
    }

    public class AnnotationRequest
    {
        public string Input { get; init; } = "";
        public string LanguageCode { get; init; } = "";
    }
}
