using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using Voartec.Models;
using Voartec.Dao;

namespace Voartec.Business
{
    public class UserPermissionBusiness
    {
        private NpgsqlConnection conn;

        public UserPermissionBusiness(NpgsqlConnection conection)
        {
            conn = conection;
        }

        public List<string> Validate(UserPermission obj, int user_id, string action)
        {
            List<string> list_erros = new List<string>();

            UserDao dao = new UserDao(conn, null);

            if (!dao.GetPermission(user_id, "user", action))
            {
                list_erros.Add("Permissão Negada.");
                return list_erros;
            }

            if (action == "update" || action == "delete")
            {
            }

            if (action == "create" || action == "update")
            {
            }

            return list_erros;
        }
    }
}