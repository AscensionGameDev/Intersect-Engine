using Intersect.Logging;
using Intersect.Server.WebApi.Authentication;
using Jose;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Linq;

namespace Intersect.Server.WebApi
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IAuthorizationProvider mAuthorizationProvider;

        public Bootstrapper(IAuthorizationProvider authorizationProvider)
        {
            mAuthorizationProvider = authorizationProvider;
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            pipelines?.AfterRequest?.AddItemToEndOfPipeline(pipelineContext =>
            {
                pipelineContext?.Response?.Headers?.Add("Access-Control-Allow-Headers", "Authorization");
                pipelineContext?.Response?.Headers?.Add("Access-Control-Allow-Origin", "*");
                pipelineContext?.Response?.Headers?.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,OPTIONS,PATCH");
                pipelineContext?.Response?.Headers?.Add("Access-Control-Request-Headers", "Authorization");
                pipelineContext?.Response?.Headers?.Add("WWW-Authenticate", "Bearer");
            });

            StatelessAuthentication.Enable(pipelines, new StatelessAuthenticationConfiguration(ctx =>
            {
                var authorizationHeader = ctx?.Request?.Headers?.Authorization;

                try
                {
                    var token = mAuthorizationProvider?.Decode(authorizationHeader);
                    var expiration = DateTime.FromBinary(token?.Expiration ?? long.MinValue);
                    return expiration <= DateTime.UtcNow ? null : mAuthorizationProvider?.FindUserFrom(token);
                }
                catch (Exception)
                {
                    return null;
                }
            }));
        }
    }
}
