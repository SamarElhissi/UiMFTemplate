namespace UiMFTemplate.Users
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
	using System.Web;
	using Microsoft.AspNetCore.Identity;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Messages;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Users.Commands;

    public static class Extensions
	{
		public static void EnforceSuccess(this IdentityResult result, string message)
		{
			if (!result.Succeeded)
			{
				throw new BusinessException(message);
			}
		}

		public static async Task EnsureRoles(this RoleManager<ApplicationRole> roleManager, params SystemRole[] roles)
		{
			await roleManager.EnsureRoles(roles.Select(t => t.Name));
		}

		public static async Task EnsureRoles(this RoleManager<ApplicationRole> roleManager, IEnumerable<string> roles)
		{
			if (roles == null)
			{
				return;
			}

			foreach (var systemRole in roles)
			{
				var exists = await roleManager.RoleExistsAsync(systemRole);

				if (!exists)
				{
					var role = new ApplicationRole
					{
						Name = systemRole,
						NormalizedName = systemRole
					};

					(await roleManager.CreateAsync(role))
						.EnforceSuccess($"Failed to ensure role '{systemRole}'");

					(await roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Role, systemRole, systemRole)))
						.EnforceSuccess($"Failed to add claim to role '{systemRole}'.");
				}
			}
		}

		public static async Task SendConfirmationEmail(
			this ApplicationUser applicationUser,
			AppConfig appConfig,
			IEmailSender emailSender,
			UserManager<ApplicationUser> userManager)
		{
			var code = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

			var confirmAccountUrl = $"{appConfig.SiteRoot}#/form/confirm-account" +
				$"?Token={HttpUtility.UrlEncode(code)}" +
				$"&Id={applicationUser.Id}";

			await emailSender.SendEmailAsync(
				applicationUser.Email,
				"Confirm your account",
				$"Please confirm your account by clicking this <a href=\"{confirmAccountUrl}\">link</a>");
		}

        public static string SendSmsCode(
            this ApplicationUser applicationUser, string code)
        {
            var message = "Your code is: " + code;
            var mobile = "966" + applicationUser.Email;
            var httpRequest =
                WebRequest.Create(
                    "http://www.4jawaly.net/api/sendsms.php?username=naol&password=123456&message=" + message + "&numbers="+ mobile + "&sender=NAOL-AD&return=Json");

                httpRequest.Method = "POST";

                var stOut = new
                    StreamWriter(httpRequest.GetRequestStream(),
                        Encoding.ASCII);
                stOut.Write("");
                stOut.Close();

                var stIn = new StreamReader(httpRequest.GetResponse().GetResponseStream());
                var strResponse = stIn.ReadToEnd();
                stIn.Close();

                return strResponse;
        }

        public static string SendSmsCode(
            string mobile, string code)
        {
            var message = "Your code is: " + code;
            var httpRequest =
                WebRequest.Create(
                    "http://www.4jawaly.net/api/sendsms.php?username=naol&password=123456&message=" + message + "&numbers=" + mobile + "&sender=NAOL-AD&return=Json");

            httpRequest.Method = "POST";

            var stOut = new
                StreamWriter(httpRequest.GetRequestStream(),
                    Encoding.ASCII);
            stOut.Write("");
            stOut.Close();

            var stIn = new StreamReader(httpRequest.GetResponse().GetResponseStream());
            var strResponse = stIn.ReadToEnd();
            stIn.Close();

            return strResponse;
        }
    }
}
