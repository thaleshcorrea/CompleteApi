//using Microsoft.AspNetCore.Mvc.ApiExplorer;
//using Microsoft.Extensions.Options;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Swashbuckle.Swagger;

//namespace DevIO.Api.Configuration
//{
//    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
//    {
//        readonly IApiVersionDescriptionProvider provider;

//        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

//        public void Configure(SwaggerGenOptions options)
//        {
//            foreach (var description in provider.ApiVersionDescriptions)
//            {
//                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
//            }
//        }

//        static Info CreateInfoForApiVersion(ApiVersionDescription description)
//        {
//            var info = new Info()
//            {
//                title = "API - desenvolvedor.io",
//                version = description.ApiVersion.ToString(),
//                description = "Esta API faz parte do curso REST com ASP.NET Core WebAPI.",
//                contact = new Contact() { name = "Eduardo Pires", email = "contato@desenvolvedor.io" },
//                termsOfService = "https://opensource.org/licenses/MIT",
//                license = new License() { name = "MIT", url = "https://opensource.org/licenses/MIT" }
//            };

//            if (description.IsDeprecated)
//            {
//                info.description += " Esta versão está obsoleta!";
//            }

//            return info;
//        }
//    }

//    public class SwaggerDefaultValues : IOperationFilter
//    {
//        public void Apply(Operation operation, OperationFilterContext context)
//        {
//            var apiDescription = context.ApiDescription;

//            operation.deprecated = apiDescription.IsDeprecated();

//            if (operation.parameters == null)
//            {
//                return;
//            }

//            foreach (var parameter in operation.parameters.OfType<NonBodyParameter>())
//            {
//                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

//                if (parameter.Description == null)
//                {
//                    parameter.Description = description.ModelMetadata?.Description;
//                }

//                if (parameter.Default == null)
//                {
//                    parameter.Default = description.DefaultValue;
//                }

//                parameter.Required |= description.IsRequired;
//            }
//        }
//    }

//    public class SwaggerAuthorizedMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public SwaggerAuthorizedMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            if (context.Request.Path.StartsWithSegments("/swagger")
//                && !context.User.Identity.IsAuthenticated)
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                return;
//            }

//            await _next.Invoke(context);
//        }
//    }
//}
