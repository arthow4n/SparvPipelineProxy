using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SparvPipelineProxy.Utils;

namespace SparvPipelineProxy.Controllers
{
    [ApiController]
    public class SparvPipelineController : ControllerBase
    {
        [HttpPost("/annotate/sparv")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AnnotationResponse))]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Annotate(AnnotationRequest request)
        {
            if (request.LanguageCode != "swe")
            {
                return Problem(statusCode: (int)HttpStatusCode.BadRequest, title: "Only Swedish (swe) is supported for now.");
            }

            return Ok(new AnnotationResponse
            {
                Xml = await CommandService.AnnotateWithSparvPipeline(request.LanguageCode, request.Input),
            });
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

    public class AnnotationResponse
    {
        public string Xml { get; init; } = "";
    }
}
