using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Dao;

namespace Voartec.Business
{
    public class RequestBusiness
    {

        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;

		public RequestBusiness(NpgsqlConnection connection, NpgsqlTransaction transaction)
		{
			conn = connection;
			tran = transaction;
		}

        /// <summary>
        /// Valida uma requisição quanto as ações que estão sendo feitas sobre ela
        /// </summary>
        /// <param name="obj">É o objeto requisição</param>
        /// <param name="user_id">É o usuário que está solicitando a operação</param>
        /// <param name="action">É o tipo de ação que está sendo feita pelo usuário</param>
        /// <returns></returns>
		public List<string> Validate(Request obj, int user_id, string action)
		{
			List<string> list_erros = new List<string>();
            RequestDao dao = new RequestDao(conn, tran);
			UserDao userDao = new UserDao(conn, tran);
			Request request = new Request();

			if (action == "reopen" || action == "finish")
			{
				if (!userDao.GetPermission(user_id, "purchase", "update")) list_erros.Add("Permissão Negada.");
			}
			else
			{
				if (!userDao.GetPermission(user_id, "purchase", action)) list_erros.Add("Permissão Negada.");
			}

			if (action == "update" || action == "delete" || action == "finish" || action == "reopen")
			{
				if (obj.req_id == 0)
				{
					list_erros.Add("O objeto informado se refere a um novo registro.");
					return list_erros;
				}

                // Verifica se o registro existe
				if (dao.GetById((int)obj.req_id).req_id == 0)
				{
					list_erros.Add("Registro não encontrado.");
					return list_erros;
				}

				request = dao.GetById(obj.req_id);
				if (action != "reopen" && request.req_status != "E") // --> Em digitação
				{
					list_erros.Add("O Status do registro não permite alterações.");
					return list_erros;
				}

				if (action == "reopen" && request.req_status != "A") // --> Ativa ou finalizada
				{
					list_erros.Add("O Status do registro não permite estorno.");
					return list_erros;
				}

				if(RequestIsEmpty(request.req_id))
				{
					list_erros.Add("A requisição não contém itens!");
					return list_erros;
				}

			}

			return list_erros;
		}

		/// <summary>
		/// Diz se uma requisição é vazia, ou seja, se não há itens adicionados
		/// </summary>
		/// <param name="request_id"></param>
		/// <returns></returns>
		private bool RequestIsEmpty(int request_id)
		{
			RequestItemDao item_dao = new RequestItemDao(conn, null);
			return (item_dao.GetItemsByRequest(request_id).Count == 0);
		}
    }
}