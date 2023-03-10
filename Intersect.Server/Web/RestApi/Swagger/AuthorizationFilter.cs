// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Web.Http;
// using System.Web.Http.Description;

// using Swashbuckle.Swagger;

// namespace Intersect.Server.Web.RestApi.Swagger
// {
//     internal class AuthorizationFilter : IOperationFilter
//     {
//         public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
//         {
//             var authorizeAttributes = apiDescription.GetControllerAndActionAttributes<AuthorizeAttribute>();
//             if (authorizeAttributes.Any())
//             {
//                 operation.responses.Add(
//                     $"{(int)HttpStatusCode.Unauthorized}",
//                     new Response { description = "Unauthorized" }
//                 );
//             }
//         }
//     }
// }
