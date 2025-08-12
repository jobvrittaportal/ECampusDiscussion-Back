using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECampusDiscussion.JwtAuth
{
    public class TokenManager : ITokenManager
    {
        private JwtSecurityTokenHandler tokenHandler;
        private byte[] secretKey;

        public TokenManager()
        {
            tokenHandler = new JwtSecurityTokenHandler();
            secretKey = Encoding.ASCII.GetBytes("Sanidhytiwarimcakitmirzamurad2001");
        }

        public string NewToken(string EmployeeID)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("EmployeeID", EmployeeID)
                }),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
