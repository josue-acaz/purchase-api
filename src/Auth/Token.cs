using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Voartec.Cryptography
{
    public class Token
    {

        public int GetIdUserToken(string tokenString)
        {

            var jwtEncodedString = tokenString.Substring(7); // trim 'Bearer ' from the start since its just a prefix for the token string

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            return Convert.ToInt32(token.Claims.First(c => c.Type == "use_id").Value);
        }

    }
}
