using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiHost.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class UploadImageRequest
    {
        public IFormFile Image { get; set; }
        public int ProductId { get; set; }
    }
    public class ProductImage
    {
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
    
    public class Customers
    {
        public int customer_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public decimal 	primary_latitude	{get; set;}
        public decimal 	primary_longitude	{get; set;}
        public string primary_address { get; set; }
        public DateTime created_at { get; set; }
    }

           // <— match your project’s root namespace

    public sealed class JwtTokenService
    {
        private readonly IConfiguration _cfg;
        public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

        public string Generate(int userId, string email)
        {
            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
                },
                expires: DateTime.UtcNow.AddMinutes(
                             int.Parse(_cfg["Jwt:ExpiresMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
