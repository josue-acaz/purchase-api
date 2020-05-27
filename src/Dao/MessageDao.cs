using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Helpers;
using Serilog;
using Newtonsoft.Json.Linq;

namespace Voartec.Dao
{
    public class MessageDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;

        // Querys
        private string post = "insert into message(msg_id, msg_user_from, msg_user_to, msg_source_type, msg_source_key, msg_text, msg_sent_date_hour, msg_sent, msg_read, msg_important, msg_excluded) values(@msg_id, @msg_user_from, @msg_user_to, @msg_source_type, @msg_source_key, @msg_text, @msg_sent_date_hour, @msg_sent, @msg_read, @msg_important, @msg_excluded);";
        private string getById = "select * from message where msg_id=@msg_id";
        private string delete = "update message set msg_excluded=true where msg_id=@msg_id;";
        private string important = "update message set msg_important=true where msg_id=@msg_id";
        private string read = "update message set msg_read=true where msg_id=@msg_id";
        private string getMessagesBySourceKey = "select * from Message where msg_source_key=@msg_source_key";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public MessageDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: cria uma nova mensagem!
        /// <reponse>: retorna o id da mensagem criada.
        public void Post(Message msg)
        {
            // Atribue o id da requisição
            SchemeChecker checker = new SchemeChecker(conn, tran);
            msg.SetId(checker.LastId("message", "msg_id") + 1);
            msg.msg_sent = true; // Mensagem foi enviada

            // Insere os dados na tabela de requisição
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@msg_id", msg.msg_id);
            cmd.Parameters.AddWithValue("@msg_user_from", msg.msg_user_from);
            cmd.Parameters.AddWithValue("@msg_user_to", msg.msg_user_to);
            cmd.Parameters.AddWithValue("@msg_source_type", msg.msg_source_type);
            cmd.Parameters.AddWithValue("@msg_source_key", msg.msg_source_key);
            cmd.Parameters.AddWithValue("@msg_text", msg.msg_text);
            cmd.Parameters.AddWithValue("@msg_sent_date_hour", msg.msg_sent_date_hour);
            cmd.Parameters.AddWithValue("@msg_sent", msg.msg_sent);
            cmd.Parameters.AddWithValue("@msg_read", msg.msg_read);
            cmd.Parameters.AddWithValue("@msg_important", msg.msg_important);
            cmd.Parameters.AddWithValue("@msg_excluded", msg.msg_excluded);
            cmd.ExecuteNonQuery();
        }

        /// <summary>: obtém uma determinada mensagem!
        /// <response>: os dados da mensagem.
        public Message GetById(int msg_id)
        {
            Message msg = new Message();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@msg_id", msg_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                msg.msg_id = (int)reader["msg_id"];
                msg.msg_user_from = (int)reader["msg_user_from"];
                msg.msg_user_to = (int)reader["msg_user_to"];
                msg.msg_source_type = (string)reader["msg_source_type"];
                msg.msg_source_key = (int)reader["msg_source_key"];
                msg.msg_text = (string)reader["msg_text"];
                msg.msg_sent_date_hour = (DateTime?)reader["msg_sent_date_hour"];
                msg.msg_read = (bool)reader["msg_read"];
                msg.msg_excluded = (bool)reader["msg_excluded"];
            }
            reader.Close();

            return msg;
        }

        /// <summary>: a mensagem é marcada como excluida, o registro permanece!
        /// <response>: id da mensagem que foi excluída.
        public int Delete(int msg_id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@msg_id", msg_id);
            cmd.ExecuteNonQuery();

            return msg_id;
        }

        /// <summary>: a mensagem é marcada como importante!
        /// <response>: id da mensagem.
        public int IsImportant(int msg_id)
        {
            cmd = new NpgsqlCommand(important, conn, tran);
            cmd.Parameters.AddWithValue("@msg_id", msg_id);
            cmd.ExecuteNonQuery();

            return msg_id;
        }

        /// <summary>: a mensagem é marcada como lida!
        /// <response>: id da mensagem.
        public int WasRead(int msg_id)
        {
            cmd = new NpgsqlCommand(read, conn, tran);
            cmd.Parameters.AddWithValue("@msg_id", msg_id);
            cmd.ExecuteNonQuery();

            return msg_id;
        }

        /// <summary>: Busca todas as mensagens associadas à um determinado recurso!
        /// <response>: uma lista contendo todas as mensagens do recurso.
        public List<Message> GetMessagesBySourceKey(int source_key)
        {
            List<Message> messages = new List<Message>();
            cmd = new NpgsqlCommand(getMessagesBySourceKey, conn);
            cmd.Parameters.AddWithValue("@msg_source_key", source_key);
            reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                Message msg = new Message();
                msg.msg_id = (int)reader["msg_id"];
                msg.msg_user_from = (int)reader["msg_user_from"];
                msg.msg_user_to = (int)reader["msg_user_to"];
                msg.msg_source_type = (string)reader["msg_source_type"];
                msg.msg_source_key = (int)reader["msg_source_key"];
                msg.msg_text = (string)reader["msg_text"];
                msg.msg_sent_date_hour = (DateTime)reader["msg_sent_date_hour"];
                msg.msg_read = (bool)reader["msg_read"];
                msg.msg_important = (bool)reader["msg_important"];
                msg.msg_excluded = (bool)reader["msg_excluded"];
                messages.Add(msg);
            }
            reader.Close();

            return messages;
        }
    }
}