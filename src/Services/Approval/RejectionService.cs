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
    public class RejectionService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        public ObjResult TechnicalRejection(int request_id, RequestItem[] items, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            RejectionBusiness rejectionBusiness = new RejectionBusiness(connection);
            List<string> messages_list = new List<string>();
            ObjResult result = new ObjResult();

            // Validar processo de rejeição
            try
            {
                messages_list = rejectionBusiness.Validate(items, user_id, action);
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
            ItemStatusDao status_dao = new ItemStatusDao(connection, transaction_01);
            RequestItemDao item_dao = new RequestItemDao(connection, transaction_01);
            RequestDao request_dao = new RequestDao(connection, transaction_01);
            // Iniciar processo de aprovação
            try
            {
                // Rejeitar item por item
                foreach (RequestItem item in items)
                {
                    // Alterar o status para 3 (Aprovação Técnica Negada - ATN)
                    status_dao.ChangeStatus(item.itm_id, 3);
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
                // Verificar se todos os itens da requisição já passaram por aprovação ou rejeição
                if(item_dao_02.TechnicalApprovalIsFinished(request_id))
                {
                    if(item_dao_02.NotApproved(request_id))
                    {
                        Console.WriteLine("Aprovação negada...");
                        requestDao.ChangeStatus(request_id, 'N');
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