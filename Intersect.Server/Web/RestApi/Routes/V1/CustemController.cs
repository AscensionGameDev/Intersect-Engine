using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using Intersect.Server.Notifications;
using Intersect.Server.Web.RestApi.Attributes;
using Intersect.Server.Web.RestApi.Extensions;
using Intersect.Server.Web.RestApi.Payloads;
using Intersect.Server.Web.RestApi.Types;
using Intersect.Utilities;

using JetBrains.Annotations;
namespace Intersect.Server.Web.RestApi.Routes.V1
{
	[RoutePrefix("custom")]
	[ConfigurableAuthorize(Roles = nameof(ApiRoles.UserQuery))]
	public class CustemController : IntersectApiController
	{
		[Route("user/mail/")]
		[HttpPost]
		public object UserByMail([FromBody] UserInfo userInfo)
		{
			if (string.IsNullOrEmpty(userInfo.Email))
			{
				return Request.CreateErrorResponse(
					HttpStatusCode.BadRequest,
					$@"Email all must be provided, and not null/empty."
				);
			}

			var user = Database.PlayerData.User.FindByMail(userInfo.Email);

			if (user == null)
			{
				return Request.CreateErrorResponse(HttpStatusCode.NotFound, $@"No user with mail '{userInfo.Email}'.");
			}

			return user;
		}
	}
}
