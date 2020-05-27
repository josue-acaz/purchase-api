using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Npgsql;
using Voartec.Dao;
using Voartec.Models;
using Voartec.Helpers;

namespace Voartec.Services
{
    public class MessageService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna uma determinada mensagem</sumary>
        public ObjResult GetMessageById(int msg_id)
        {
            connection = db.GetCon();
            connection.Open();

            Message msg = new Message();
            ObjResult result = new ObjResult();
            MessageDao dao = new MessageDao(connection, null);

            try
            {
                msg = dao.GetById(msg_id);
                result.SetData(msg);
                result.Success();
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Envia uma mensagem</sumary>
        public void SendMessage(Message msg)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            MessageDao dao = new MessageDao(connection, transaction);

            try
            {
                dao.Post(msg);
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
        }

        /// <summary>Exclui uma determinada mensagem</sumary>
        public ObjResult DeleteMessage(int msg_id)
        {
            connection = db.GetCon();
            connection.Open();
            NpgsqlTransaction transaction = connection.BeginTransaction();
            MessageDao dao = new MessageDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                msg_id = dao.Delete(msg_id);
                result.SetData(msg_id);
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

        /// <summary>Marca como importante uma determinada mensagem</sumary>
        public ObjResult MarkAsImportant(int msg_id)
        {
            connection = db.GetCon();
            connection.Open();

            MessageDao dao = new MessageDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                msg_id = dao.IsImportant(msg_id);
                result.SetData(msg_id);
                result.Success();
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Marcar como lida uma determinada mensagem</sumary>
        public ObjResult MarkAsRead(int msg_id)
        {
            connection = db.GetCon();
            connection.Open();

            MessageDao dao = new MessageDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                msg_id = dao.WasRead(msg_id);
                result.SetData(msg_id);
                result.Success();
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Retorna as mensagens associadas à um determinado recurso</sumary>
        public List<Message> GetMessagesBySourceKey(int source_key)
        {
            connection = db.GetCon();
            connection.Open();

            List<Message> messages = new List<Message>();
            MessageDao dao = new MessageDao(connection, null);

            try
            {
                messages = dao.GetMessagesBySourceKey(source_key);
            }
            finally
            {
                connection.Close();
            }

            return messages;
        }
    }
}