using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Net7_Api_Demo.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Net7_Api_Demo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <summary>
        /// 取得API的Token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetJwtToken(LoginModel data)
        {
            List<Claim> claims = new List<Claim>();
            //使用者基本資訊
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, data.Account));
            //使用者角色權限資訊
            claims.Add(new Claim(ClaimTypes.Role, "TestRole"));

            //取出appsettings.json裡的KEY處理
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

            //設定jwt資訊
            var jwt = new JwtSecurityToken
            (
                issuer: _configuration["JWT:Issuer"],     //此Token的發行人
                audience: _configuration["JWT:Audience"], //允許的使用者
                claims: claims,                           //Claim中設定的使用者基本資訊
                expires: DateTime.Now.AddMinutes(2),      //此Token的有效時間 (設定1分鐘)
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256) //此Token的加密方式
            );

            //產生JWT Token
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }

        /// <summary>
        /// 取得自己的帳號名稱
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "TestRole")]
        public string GetAccount()
        {
            //取得登入時設定在Claim中的Account (注意：Claim中的資料寫入時是宣告哪種型別，就必須以該型別去取資料)
            string accountName = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Name);
            return accountName;
        }
    }
}
