using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Npgsql;
using Voartec.Dao;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Business;

namespace Voartec.Services
{
    public class ApprovalService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>
        /// Faz a aprovação de uma requisição e seus itens
        /// </summary>
        /// <param name="request_id">É o id da requisição</param>
        /// <param name="items">É o array de itens da requisição</param>
        /// <param name="user_id">É o id do usuário que está realizando a operação</param>
        /// <param name="action">É o tipo de ação que está sendo executada</param>
        /// <returns></returns>
        public ObjResult TechnicalApproval(int request_id, RequestItem[] items, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            ApprovalBusiness approvalBusiness = new ApprovalBusiness(connection);
            List<string> messages_list = new List<string>();
            ObjResult result = new ObjResult();

            // Validar processo de aprovação
            try
            {
                messages_list = approvalBusiness.Validate(items, user_id, action);
                if(messages_list.Count > 0)
                {
                    result.resultStatus = "error";
                    result.resultMessages = messages_list;
                    connection.Close();
                    return result;
                }
            }
            finally
            {
                connection.Close();
            }

            connection.Open();
            NpgsqlTransaction transaction_01 = connection.BeginTransaction();
            ItemStatusDao statusDao = new ItemStatusDao(connection, transaction_01);
            RequestItemDao item_dao_01 = new RequestItemDao(connection, transaction_01);

            // Inciar processo de aprovação
            try
            {
                // Aprovar item por item
                foreach (RequestItem item in items)
                {
                    // Alterar o status para 4 (Aprovação Técnica Concedida - ATC)
                    statusDao.ChangeStatus(item.itm_id, 4);
                    // Se a quantidade solicitada for diferente da quantidade aprovada, atualizar item
                    if(item.itm_quantity != item.itm_approved_quantity)
                    {
                        Console.WriteLine("A quantidade aprovada é diferente da quantidade solicitada!");
                        item_dao_01.SetApprovedQuantity(item.itm_id, item.itm_approved_quantity);
                    }
                }

                result.resultStatus = "success";
                result.data = request_id;
                transaction_01.Commit();
            }
            catch (Exception e)
            {
                transaction_01.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            connection.Open();
            NpgsqlTransaction transaction_02 = connection.BeginTransaction();
            RequestItemDao item_dao_02 = new RequestItemDao(connection, transaction_02);
            RequestDao requestDao = new RequestDao(connection, transaction_02);
            try
            {
                // Verificar se todos os itens da requisição já passaram por aprovação
                if(item_dao_02.TechnicalApprovalIsFinished(request_id))
                {
                    Console.WriteLine("Todos os itens já passaram por aprovação...");
                    
                    // Alterar o status da requisição para 
                    if(item_dao_02.FullApproved(request_id))
                    {
                        Console.WriteLine("Aprovação completa...");
                        requestDao.ChangeStatus(request_id, 'C');
                    }

                    if(item_dao_02.PartiallyApproved(request_id))
                    {
                        Console.WriteLine("Aprovação parcial...");
                        requestDao.ChangeStatus(request_id, 'P');
                    }
                }

                result.resultStatus = "success";
                result.data = request_id;
                transaction_02.Commit();
            }
            catch (Exception e)
            {
                transaction_02.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
    }
}