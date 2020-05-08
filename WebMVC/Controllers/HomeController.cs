using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult GetPrivateDatas()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var culture = CultureInfo.InvariantCulture;
            IList<Claim> claim = identity.Claims.ToList();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim[3].Value));
            var TokenLifetime = dateTimeOffset.LocalDateTime.ToString(culture);
            return Ok(new { CurrentTime = DateTime.Now.ToString(culture), TokenLifetime, UserName = claim.Count > 0 ? claim[0].Value ?? "" : "" });
        }
    }
}
