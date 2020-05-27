using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Dao;

namespace Voartec.Business
{
    public class ApprovalBusiness
    {
        private NpgsqlConnection conn;

        /// <summary>
        /// Método construtor que iniciliza com a conexão do banco de dados
        /// </summary>
        /// <param name="conection">É a instância de conexão da classe de serviços</param>
        public ApprovalBusiness(NpgsqlConnection conection)
        {
            conn = conection;
        }

        /// <summary>
        /// Valida a aprovação atual
        /// </summary>
        /// <param name="items">Itens a serem aprovados</param>
        /// <param name="user_id">ID do usuário que está realizando a operação</param>
        /// <param name="action">Tipo de ação que está sendo executada</param>
        /// <returns></returns>
        public List<string> Validate(RequestItem[] items, int user_id, string action)
        {
            List<string> list_erros = new List<string>();
            UserDao userDao = new UserDao(conn, null);

            // Verifica se o usuário tem permissão para aprovar os itens
            if (!userDao.GetPermission(user_id, "technical_authorization", action))
            {
                list_erros.Add("Permissão Negada.");
                return list_erros;
            }

            // Verifica se os status dos itens permitem aprovação
            if(!IsAwaitingTechnicalApproval(items))
            {
                list_erros.Add("Houve um erro ao aprovar os itens solicitados.");
            }

            return list_erros;
        }

        // Verifica se o status atual do item é 2 (Aguardando Aprovação Técnica)
        private bool IsAwaitingTechnicalApproval(RequestItem[] items)
        {
            bool isAwaiting = false;
            ItemStatusDao statusDao = new ItemStatusDao(conn, null);

            foreach(RequestItem item in items)
            {
                // Se o status atual do item for 2, então ele está AAT
                if(statusDao.CurrentStatus(item.itm_id) == 2)
                {
                    isAwaiting = true;
                }
                // Caso contrário, se existir algum item que não tenha esse status, retornar falso
                else { return false; }
            }

            return isAwaiting;
        }
        
    }
}