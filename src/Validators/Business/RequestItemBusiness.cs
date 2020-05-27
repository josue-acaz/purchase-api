using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Dao;

namespace Voartec.Business
{
    public class RequestItemBusiness
    {
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;

		public RequestItemBusiness(NpgsqlConnection connection, NpgsqlTransaction transaction)
		{
			conn = connection;
			tran = transaction;
		}

        public List<string> Validate(RequestItem obj, int user_id, string action)
		{
            Request request = new Request();
            RequestDao requestDao = new RequestDao(conn, tran);
            RequestItemDao requestItemDao = new RequestItemDao(conn, tran);
            UserDao userDao = new UserDao(conn, tran);

			List<string> list_erros = new List<string>();

            // Verificar permissão do usuário para realizar a operação
			if (!userDao.GetPermission(user_id, "purchase", action))
			{
				list_erros.Add("Permissão Negada.");
				return list_erros;
			}

            // Verificar se a ação for criar, atualizar ou deletar, buscar a requisição referente ao item
			if (action == "create" || action == "update" || action == "delete")
			{
				request = requestDao.GetById(obj.itm_request_id);

				if(request.req_status != "E")
				{
					list_erros.Add("Não é possível efetuar a ação, pois o status da requisição não permite.");
				}
			}
				
            // Verificar se a atualização ou exclusão é possível para o item
			if (action == "update" || action == "delete")
			{
                // É um novo registro
				if (obj.itm_id == 0)
				{
					list_erros.Add("O objeto informado se refere a um novo registro.");
					return list_erros;
				}
                // Não foi encontrado o registro
				if (requestItemDao.GetById((int)obj.itm_id).itm_id == 0)
				{
					list_erros.Add("Registro não encontrado.");
					return list_erros;
				}
			}

            return list_erros;
		}
    }
}