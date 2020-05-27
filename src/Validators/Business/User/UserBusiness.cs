using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Dao;

namespace Voartec.Business
{
    public class UserBusiness
    {
        private NpgsqlConnection conn;

        /// <summary>
        /// Método construtor que iniciliza com a conexão do banco de dados
        /// </summary>
        /// <param name="conection">É a instância de conexão da classe de serviços</param>
        public UserBusiness(NpgsqlConnection conection)
        {
            conn = conection;
        }

        /// <summary>
        /// Valida um usuário
        /// </summary>
        /// <param name="user">é o objeto usuário</param>
        /// <param name="user_id">é o id do usuário</param>
        /// <param name="action">é o tipo de ação que está sedo executada</param>
        /// <returns>lista de erros, se existirem</returns>
        public List<string> Validate(User user, int user_id, string action)
        {
            List<string> list_erros = new List<string>();
            UserDao userDao = new UserDao(conn, null);


            if (!userDao.GetPermission(user_id, "user", action))
            {
                list_erros.Add("Permissão Negada.");
                return list_erros;
            }

            if (action == "update" || action == "delete")
            {
                if (userDao.GetById(user.use_id) == null)
                {
                    if (userDao.GetById(user.use_id).use_id == 0)
                    {
                        list_erros.Add("Registro não encontrado.");
                        return list_erros;
                    }
                }
            }

            if (action == "create" || action == "update")
            {
                if (user.use_name.Length == 0 || user.use_name.Length > 40)
                {
                    list_erros.Add("Preencha do campo NOME corretamente.");
                }
            }

            return list_erros;
        }

        /// <summary>
        /// Valida o perfil de um usuário
        /// </summary>
        /// <param name="profile">é o objeto profile</param>
        /// <param name="user_id">é o id do usuário</param>
        /// <param name="action">é o tipo de ação que está sendo executada</param>
        /// <returns>lista de erros, se existirem</returns>
        public List<string> ValidatePerfil(UserProfile profile, int user_id, string action)
        {
            List<string> list_erros = new List<string>();

            UserDao userDao = new UserDao(conn, null);


            if (userDao.GetById(profile.use_id) == null) list_erros.Add("Registro não encontrado.");

            if (action == "update")
            {
                if (profile.use_name == "") list_erros.Add("Preencha do campo NOME corretamente.");
                if (profile.use_code == "") list_erros.Add("Preencha do campo Codigo corretamente.");
                if (profile.use_full_name == "") list_erros.Add("Preencha do campo Nome Completo corretamente.");
            }

            return list_erros;
        }
    }
}