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
    public class RequestItemService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna um determinado item</sumary>
        public ObjResult GetById(int item_id)
        {
            connection = db.GetCon();
            connection.Open();

            RequestItem item = new RequestItem();
            ObjResult result = new ObjResult();
            RequestItemDao dao = new RequestItemDao(connection, null);

            try
            {
                item = dao.GetById(item_id);
                result.SetData(item);
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
        public ObjResult Delete(int item_id)
        {
            connection = db.GetCon();
            connection.Open();

            RequestItemDao dao = new RequestItemDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                item_id = dao.Delete(item_id);
                result.SetData(item_id);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Cria um novo item e atribui à uma requisição</sumary>
        public ObjResult Post(RequestItem item, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            RequestItemBusiness business = new RequestItemBusiness(connection, null);
            List<string> messages_list = new List<string>();
            ObjResult result = new ObjResult();

            try
            {
                messages_list = business.Validate(item, user_id, action);
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
            RequestItemDao dao = new RequestItemDao(connection, transaction);

            try
            {
                item.SetId(dao.Post(item));
                transaction.Commit();
                result.Success();
                result.SetData(item.GetId());
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

        /// <summary>Atualiza um item</sumary>
        public ObjResult Update(int item_id, RequestItem item, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            RequestItemBusiness business = new RequestItemBusiness(connection, null);
            List<string> messages_list = new List<string>();
            ObjResult result = new ObjResult();

            try
            {
                item.SetId(item_id); // <-- Assegura que o item possua um id para atualização
                messages_list = business.Validate(item, user_id, action);
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
            RequestItemDao dao = new RequestItemDao(connection, transaction);

            try
            {
                item_id = dao.Update(item_id, item);
                transaction.Commit();
                result.Success();
                result.SetData(item_id);
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

        /// <summary>Cancela um determinado item</sumary>
        public ObjResult Cancel(int item_id)
        {
            connection = db.GetCon();
            connection.Open();

            RequestItemDao dao = new RequestItemDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                item_id = dao.Cancel(item_id);
                result.SetData(item_id);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Define o status de um determinado item</sumary>
        public ObjResult SetStatus(int item_id, string status_name)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            RequestItemDao dao = new RequestItemDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                item_id = dao.SetStatus(item_id, status_name);
                transaction.Commit();
                result.Success();
                result.SetData(item_id);
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

        /// <summary>Faz a listagem dos itens</sumary>
        public PaginationResult List(string param)
        {
            connection = db.GetCon();
            connection.Open();

            PaginationResult pagination = new PaginationResult();
            RequestItemDao dao = new RequestItemDao(connection, null);

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

        /// <summary>Retorna as dependências de que existem do item para com a requisição</sumary>
        /// <values>A prioridade, a data limite e a aplicação</values>
        public ObjResult GetRequestDependencies(int request_id)
        {
            connection = db.GetCon();
            connection.Open();

            Request request = new Request();
            ObjResult result = new ObjResult();
            RequestItemDao dao = new RequestItemDao(connection, null);

            try
            {
                request = dao.GetRequestDependencies(request_id);
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

        /// <summary>Retorna os itens que estão associados à uma determinada requisição</sumary>
        public List<RequestItem> GetItemsByRequest(int request_id)
        {
            connection = db.GetCon();
            connection.Open();

            List<RequestItem> items = new List<RequestItem>();
            RequestItemDao dao = new RequestItemDao(connection, null);

            try
            {
                items = dao.GetItemsByRequest(request_id);
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return items;
        }
    }
}
