using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Npgsql;
using Voartec.Dao;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Services;
using Voartec.Business;

namespace Voartec.Services
{
    public class RequestService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna uma determinada requisição</sumary>
        public ObjResult GetById(int request_id)
        {
            connection = db.GetCon();
            connection.Open();

            Request request = new Request();
            ObjResult result = new ObjResult();
            RequestDao dao = new RequestDao(connection, null);

            try
            {
                request = dao.GetById(request_id);
                result.SetData(request);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Exclui uma determinada requisição</sumary>
        public ObjResult Delete(int request_id)
        {
            connection = db.GetCon();
            connection.Open();
            ObjResult result = new ObjResult();
            List<RequestItem> items = new List<RequestItem>();
            List<Message> messages = new List<Message>();

            // Excluir a requisição
            NpgsqlTransaction transaction = connection.BeginTransaction();
            RequestDao dao = new RequestDao(connection, transaction);
            RequestItemDao itemDao = new RequestItemDao(connection, transaction);
            MessageDao messageDao = new MessageDao(connection, transaction);
            try
            {
                // Verifica se a requisição possui itens
                items = itemDao.GetItemsByRequest(request_id);
                if(items.Count > 0)
                {
                    // Exclui todos os itens que pertencem à requisição
                    foreach (RequestItem item in items)
                    {
                        itemDao.Delete(item.itm_id);
                    }
                }

                // Verifica se a requisição possui mensagens
                messages = messageDao.GetMessagesBySourceKey(request_id);
                if(messages.Count > 0)
                {
                    // Exclui todas as mensagens associadas à requisição
                    foreach(Message msg in messages)
                    {
                        messageDao.Delete(msg.msg_id);
                    }
                }

                request_id = dao.Delete(request_id);
                result.SetData(request_id);
                result.Success();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Cria uma nova requisição</sumary>
        public ObjResult Post(Request request)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            RequestDao dao = new RequestDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                request.SetId(dao.Post(request));
                transaction.Commit();
                result.Success();
                result.SetData(request.GetId());
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Atualiza uma requisição</sumary>
        public ObjResult Update(int request_id, Request request)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            RequestDao dao = new RequestDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                request_id = dao.Update(request_id, request);
                transaction.Commit();
                result.Success();
                result.SetData(request_id);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Faz a listagem das requisições</sumary>
        public PaginationResult List(string param)
        {
            connection = db.GetCon();
            connection.Open();

            PaginationResult pagination = new PaginationResult();
            RequestDao dao = new RequestDao(connection, null);

            try
            {
                pagination = dao.List(param);
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return pagination;
        }

        /// <summary>Cancela uma determinada requisição</sumary>
        public ObjResult Cancel(int request_id)
        {
            connection = db.GetCon();
            connection.Open();

            RequestDao dao = new RequestDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                request_id = dao.Cancel(request_id);
                result.SetData(request_id);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Muda o status de uma determinada requisição</sumary>
        private int ChangeStatus(int request_id, char new_status)
        {
            connection = db.GetCon();
            connection.Open();
            NpgsqlTransaction transaction = connection.BeginTransaction();

            RequestDao dao = new RequestDao(connection, null);

            try
            {
                request_id = dao.ChangeStatus(request_id, new_status);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return request_id;
        }

        /// <summary>Retorna a aplicação de uma determinada requisição</sumary>
        public string GetApplication(int request_id)
        {
            string request_application;
            connection = db.GetCon();
            connection.Open();

            RequestDao dao = new RequestDao(connection, null);

            try
            {
                request_application = dao.GetApplication(request_id);
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return request_application;
        }

        /// <summary>Retorna a prioridade de uma determinada requisição</sumary>
        public string GetPriority(int request_id)
        {
            string request_priority;
            connection = db.GetCon();
            connection.Open();

            RequestDao dao = new RequestDao(connection, null);

            try
            {
                request_priority = dao.GetPriority(request_id);
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return request_priority;
        }

        /// <summary>Faz a publicação de uma determinada requisição</sumary>
        public ObjResult Send(int request_id, int user_id, string action, Message msg)
        {
            connection = db.GetCon();
            connection.Open();

            Request request = new Request();
            ObjResult result = new ObjResult();
            List<string> messages_list = new List<string>();
            RequestDao requestDao = new RequestDao(connection, null);
            RequestBusiness business = new RequestBusiness(connection, null);

            try
            {
                // Validar requisição
                request = requestDao.GetById(request_id);
                Console.WriteLine("Status da requisição: " + request.req_status);
                messages_list = business.Validate(request, user_id, action);
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
            NpgsqlTransaction transaction = connection.BeginTransaction();
            RequestItemDao itemDao = new RequestItemDao(connection, transaction);
            MessageDao messageDao = new MessageDao(connection, transaction);
            RequestDao r_dao = new RequestDao(connection, transaction);
            try
            {
                // Altera o status da requisição para ativa
                request_id = r_dao.ChangeStatus(request_id, 'A');

                // Altera o status de todos os itens para <AAT> Aguardando Aprovação Técnica
                foreach (RequestItem item in itemDao.GetItemsByRequest(request_id))
                {
                    itemDao.SetStatus(item.itm_id, "AAT");
                }

                // Envia mensagem e informa que foi gerada por meio de uma requisição de compra
                msg.SetSourceKey(request_id);
                msg.SetSourceType("request");
                messageDao.Post(msg);

                // Adiciona o status de sucesso e data
                result.Success();
                result.SetData(request_id);

                // Comita as alterações
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Reabre uma determinada requisição
        /// </summary>
        /// <param name="request_id">É o id da requisição</param>
        /// <param name="user_id">É o id do usuário que está solicitando a reabertura da requisição</param>
        /// <param name="action">É o tipo de ação que está sendo feita pelo usuário</param>
        /// <returns></returns>
        public ObjResult ReopenRequest(int request_id, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            ObjResult result = new ObjResult();
            List<string> messages_list = new List<string>();
            Request request = new Request();

            RequestDao dao = new RequestDao(connection, null);
            RequestBusiness business = new RequestBusiness(connection, null);
            RequestItemDao item_dao = new RequestItemDao(connection, null);

            // Fazer a validação primeiro da requisição, se pode ser revertida, se o usuário tem autorização para realizar essa ação etc.
            try
            {
                request = dao.GetById(request_id);
                messages_list = business.Validate(request, user_id, action);
                
                if (messages_list.Count > 0)
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

            // Agora vem a ação de reabrir a requisição
            // Obs.: As transações são feitas separadamente para cada serviço chamado.
            try
            {
                // Excluir as mensagens que estiverem associadas à requisição
                MessageService msg_service = new MessageService();
                List<Message> messages = new List<Message>();
                messages = msg_service.GetMessagesBySourceKey(request_id);

                if(messages.Count != 0)
                {
                    foreach(Message msg in messages)
                    {
                        msg_service.DeleteMessage(msg.msg_id);
                    }
                }

                // Alterar o status de todos os itens da requisição para 1 <ED>
                foreach (RequestItem item in item_dao.GetItemsByRequest(request_id))
                {
                    item_dao.SetStatus(item.itm_id, "ED");
                }

                // Altera o status da requisição para 'E' (Em digitação).
                request_id = ChangeStatus(request_id, 'E');
                result.resultStatus = "success";
                result.data = request_id;
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            return result;
        }
    }
}
