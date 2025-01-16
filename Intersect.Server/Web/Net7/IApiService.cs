using Intersect.Core;
using Intersect.Server.Web.Configuration;

namespace Intersect.Server.Web;

public interface IApiService : IApplicationService
{
    ApiConfiguration Configuration { get; }
}