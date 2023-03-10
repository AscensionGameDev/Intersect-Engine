// using System;
// using System.Web.Http;
// using Intersect.Server.Web.RestApi.Swagger;
// using Swashbuckle.Application;

// namespace Intersect.Server.Web.RestApi
// {
//     internal static class SwaggerIsolation
//     {
//         public static void ConfigureSwagger(HttpConfiguration config)
//         {
//             config.EnableSwagger(swaggerConfig =>
//             {
//                 swaggerConfig.MultipleApiVersions(
//                     (description, version) => true,
//                     versionBuilder => versionBuilder.Version("v1", "Intersect v1 REST API")
//                 );

//                 swaggerConfig.OperationFilter<AuthorizationFilter>();
//             }).EnableSwaggerUi(swaggerUi =>
//             {
//                 swaggerUi.SupportedSubmitMethods(Array.Empty<string>());
//             });
//         }
//     }
// }
