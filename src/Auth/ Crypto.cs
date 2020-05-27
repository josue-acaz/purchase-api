using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Voartec.Cryptography
{
    public class Crypto
    {
        ///<sumary>: Gera o hash MD5
        public static string HashMD5(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(bytes);

            var ret = string.Empty;
            for(int i=0; i<hash.Length; i++)
            {
                ret += hash[i].ToString("x2");
            }

            return ret;
        }

        ///<sumary>: Retorna o id do usuário através do token de autenticação
        public int GetUserIdToken(string encoded_token)
        {
            // trim 'Bearer ' from the start since its just a prefix for the token string
            var jwtEncodedString = encoded_token.Substring(7);
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            return Convert.ToInt32(token.Claims.First(c => c.Type == "use_id").Value);
        }

        /// <summary>: Gera uma chave codificada para o usuário
        public string GenerateKey()
        {
            int length_key = 70;
            string validar = "abcdefghijklmnopqrstuvxzABCDEFGHIJKLMNOPQRSTUVXZ1234567890";
            StringBuilder strbld = new StringBuilder(50);
            Random random = new Random();
            while (0 < length_key--)
            {
                strbld.Append(validar[random.Next(validar.Length)]);
            }
            return strbld.ToString();
        }
    }
}