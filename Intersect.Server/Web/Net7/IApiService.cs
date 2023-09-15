using Intersect.Core;
using Intersect.Server.Web.RestApi.Configuration;

namespace Intersect.Server.Web;

public interface IApiService : IApplicationService
{
    ApiConfiguration Configuration { get; }
}