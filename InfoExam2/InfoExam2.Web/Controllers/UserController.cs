using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using InfoExam2.Core;
using InfoExam2.Infrastructure;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InfoExam2.Web.Controllers
{
	/// /// <summary>
	///	This class collects information about users.
	/// </summary>
	[Route("[controller]/[action]")]
	public class UsersController : Controller
	{
		private readonly ApplicationDbContext _context;

		public UsersController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public void Add(string login, string password) => _context.Users.Add(new User()
			{Login = login, Password = password});

		[HttpGet]
		public bool Contains(string login) =>
			_context.Users.FirstOrDefault(x => x.Login == login) != null;

		[HttpGet]
		public bool Check(string login, string password)
		{
			var user = _context.Users.FirstOrDefault(x => x.Login == login);
			if (user == null) return false;
			return user.Password == password;
		}

		[HttpPost]
		public IActionResult Authorize(string login, string password)
		{
			if (!Check(login, password))
				return StatusCode(500, "Incorrect user credentials!");
			
			var identity = GetIdentity(login);
			if (identity == null)
				return StatusCode(500);
			
			HttpContext.Request.Headers["Authorization"] = $"Cookie {login}:{password}";
//			FormsAuthentication.SetAuthCookie(model.Name, true);
//			var key = AuthentificationOptions.GetSymmetricSecurityKey();
//			var now = DateTime.UtcNow;
//			var jwt = new JwtSecurityToken(
//				AuthentificationOptions.Issuer,
//				AuthentificationOptions.Audience,
//				identity.Claims,
//				now,
//				now.AddMinutes(AuthentificationOptions.Lifetime),
//				new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
//			var token = new JwtSecurityTokenHandler().WriteToken(jwt);
//			Response.Headers["Authorization"] = $"Bearer {token}";
//			Response.Cookies.Append("token", token);
			return View("/Pages/Index.cshtml");
//return View("")
		}

		[HttpPost]
		public IActionResult Register(string login, string password)
		{
			_context.Users.Add(new User
			{
				Login = login,
				Password = password
			});
			_context.SaveChanges();
			var claimsIdentity = GetIdentity(login);
			var key = AuthentificationOptions.GetSymmetricSecurityKey();
			var now = DateTime.UtcNow;
			var jwt = new JwtSecurityToken(
				AuthentificationOptions.Issuer,
				AuthentificationOptions.Audience,
				claimsIdentity.Claims,
				now,
				now.AddMinutes(AuthentificationOptions.Lifetime),
				new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
			var token = new JwtSecurityTokenHandler().WriteToken(jwt);
			Response.Headers["Authorization"] = $"Bearer {token}";
			return View("Index");
		}

		private ClaimsIdentity GetIdentity(string login)
		{
			var user = Get(login);
			var claims = new List<Claim>
			{
				new Claim("Name", user.Login)
			};
			if (user.Admin)
				claims.Add(new Claim("admin", bool.TrueString));

			var claimsIdentity =
				new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
					ClaimsIdentity.DefaultRoleClaimType);
			return claimsIdentity;
		}
		public User Get(string login) => _context.Users.FirstOrDefault(x => x.Login == login) ?? null;
	}
}