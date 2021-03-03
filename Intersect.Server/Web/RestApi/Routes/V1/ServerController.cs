using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Intersect.Server.Extensions;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("server")]
    [ConfigurableAuthorize]
    public sealed class ServerController : IntersectApiController
    {

        [Route("set/exprate")]
        [HttpPost]
        public object GlobalExpRateSet([FromBody] ExpRatePayload ratePayload)
        {
            if (ratePayload.Rate > 0.0)
            {
                Options.GlobalEXPModifier = ratePayload.Rate;
                return new
                {
                    rate = Options.GlobalEXPModifier
                };
            }
            else
            {
                return Request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                $@"Global EXP Rate Modifier must be Above 0.0, you set ('{ratePayload.Rate}')."
            );
            }
        }

    }
}
