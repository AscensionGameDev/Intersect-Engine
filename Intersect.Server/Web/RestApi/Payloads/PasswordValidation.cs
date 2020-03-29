using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct PasswordValidation
    {

        [NotNull]
        public string Password { get; set; }

    }

}
