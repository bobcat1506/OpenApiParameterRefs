using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NSwag;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OpenApiParameterRefs.Controllers
{
    [ApiController]
    [Route("/sd-correspondence/correspondencev2")]
    public class CorrespondenceController : ControllerBase
    {
        [HttpPost, Route("correspondence/correspondence-operating-session/outbound/initiation")]
        [OpenApiOperation("Uploading documents for ESignature")]
        [CreateESignCorrespondenceOperationProcessor]
        [SwaggerResponse(StatusCodes.Status201Created, null, Description = "Success")]
        public IActionResult CreateESignCorrespondence([FromHeader] string accountNumber, CancellationToken cancellationToken)
        {
            // parse multi part form data much like this example
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0

            // in our actual system this is all DI'ed in
            // this doesnt need to actually run because the issue is with the Swagger UI not the execution of the service
            IMultipartFormDataParsingService service = null;

            var result = service.Parse(this.HttpContext.Request);

            // do our business logic stuff here
            // .....

            return Created("", null);
        }
    }

    public class CreateESignCorrespondenceOperationProcessorAttribute : OpenApiOperationProcessorAttribute
    {
        public CreateESignCorrespondenceOperationProcessorAttribute() : base(typeof(CreateESignCorrespondenceOperationProcessor))
        { }
    }

    public class CreateESignCorrespondenceOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var data = context.OperationDescription.Operation.Parameters;

            var schema = JsonSchema.FromType<CreateESignRequest>();

            data.Add(new OpenApiParameter()
            {
                Name = "CreateESignRequest",
                IsRequired = true,
                Kind = OpenApiParameterKind.FormData,
                Schema = schema
            });

            data.Add(new OpenApiParameter()
            {
                Name = "Document1",
                IsRequired = true,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.File,
                Description = "The first document"
            });

            data.Add(new OpenApiParameter()
            {
                Name = "Document2",
                IsRequired = false,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.File,
                Description = "The second document"
            });

            data.Add(new OpenApiParameter()
            {
                Name = "Document[n]",
                IsRequired = false,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.File,
                Description = "The n-th document"
            });

            return true;
        }
    }

    public class CreateESignRequest
    {
        public string PackageName { get; set; }

        public List<Signer> Signers { get; set; }

        public List<Document> Documents { get; set; }
    }

    public class Signer
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }

    public class Document
    {
        public string Filename { get; set; }

        public string Description { get; set; }

        public List<SignatureBlock> SignatureBlocks { get; set; }
    }

    public class SignatureBlock
    {
        public Guid Signerid { get; set; }

        public int PageNumber { get; set; }

        public string Location { get; set; }
    }

    public interface IMultipartFormDataParsingService
    {
        public MultipartFormDataParseResult Parse(HttpRequest httpRequest);
    }

    public class MultipartFormDataParseResult
    {
        public CreateESignRequest ESignRequest { get; set; }

        public List<Stream> Documents { get; set; }
    }
}
