using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Dapper;                    //  ←  needed for QueryFirstOrDefaultAsync / ExecuteScalarAsync
using Microsoft.Data.SqlClient;
using ApiHost;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;

    public AuthController(IConfiguration cfg)
    {
        _cfg = cfg;
    }


    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(
            dto.IdToken,
            new() { Audience = new[] { _cfg["Google:ClientId"] } });

        int customerId;

        const string selectSql = @"SELECT customer_id 
                               FROM customers 
                               WHERE email = @Email";

        const string insertSql = @"INSERT INTO customers
                               (name, email, created_at)
                               VALUES (@Name, @Email, NOW())
                               RETURNING customer_id";

        customerId = await DbHelper.QueryFirstOrDefaultAsync<int?>(selectSql,
                        new { Email = payload.Email }) ?? 0;

        if (customerId == 0)
        {
            customerId = await DbHelper.ExecuteScalarAsync<int>(insertSql,
                        new { Name = payload.Name, Email = payload.Email });
        }

        var token = BuildJwt(customerId, payload.Email);
        return Ok(new { token });
    }



    /*    [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            // 1️⃣  Validate Google ID‑token
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                dto.IdToken,
                new() { Audience = new[] { _cfg["Google:ClientId"] } });

            // 2️⃣  Lookup / insert customer
            int customerId;

            const string selectSql = @"SELECT customer_id 
                                       FROM   customers 
                                       WHERE  email = @Email";

            const string insertSql = @"INSERT INTO customers
                                       (name, email, created_at)
                                       OUTPUT INSERTED.customer_id
                                       VALUES (@Name, @Email, SYSUTCDATETIME())";

            //await using var conn = new SqlConnection(_cfg.GetConnectionString("Default"));

            customerId = await DbHelper.QueryFirstOrDefaultAsync<int?>(selectSql,
                            new { Email = payload.Email }) ?? 0;

            if (customerId == 0)
            {
                customerId = await  DbHelper.ExecuteScalarAsync<int>(insertSql,
                            new { Name = payload.Name, Email = payload.Email });
            }

            // 3️⃣  Generate and return your JWT
            var token = BuildJwt(customerId, payload.Email);
            return Ok(new { token });   
        }*/

    // helper
    private string BuildJwt(int id, string email)
    {
        var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
            },
            expires: DateTime.UtcNow.AddMinutes(
                        int.Parse(_cfg["Jwt:ExpiresMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public record GoogleLoginDto(string IdToken);
