using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Helpers;
using Serilog;

namespace Voartec.Dao
{
    public class ItemProgressDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;

        // Querys
        private string getById = "select * from item_progress where itp_id=@itp_id;";
        private string post = "insert into item_progress(itp_id, itp_description, itp_date_hour, itp_user_id, itp_item_id, itp_item_status_id, itp_active) values (@itp_id, @itp_description, @itp_date_hour, @itp_user_id, @itp_item_id, @itp_item_status_id, @itp_active);";
        private string delete = "delete from item_progress where itp_id=@itp_id;";
        private string update = "update item_progress set itp_description=@itp_description;";
        //private string enable = "update item_progress set itp_active=true";
        //private string disable = "update item_progress set itp_active=false";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public ItemProgressDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: obtém um determinado progresso de item!
        /// <response>: os dados do progresso do item.
        public ItemProgress GetById(int item_progress_id)
        {
            ItemProgress progress = new ItemProgress();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@itp_id", item_progress_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                progress.itp_id = (int)reader["itp_id"];
                progress.itp_description = (string)reader["itp_description"];
                progress.itp_date_hour = (DateTime?)reader["itp_date_hour"];
                progress.itp_user_id = (int)reader["itp_user_id"];
                progress.itp_item_id = (int)reader["itp_item_id"];
                progress.itp_item_status_id = (int)reader["itp_item_status_id"];
                progress.itp_active = (bool)reader["itp_active"];
            }
            reader.Close();

            return progress;
        }

        /// <summary>: cria um novo progresso para o item e o associa ao usuário que efetuou a ação, ao item e ao status do item!
        /// <response>: retorna o id do progresso criado.
        public int Post(ItemProgress progress, int user_id, int item_id, int item_status_id)
        {
            // Atribue o id do item
            SchemeChecker checker = new SchemeChecker(conn, tran);
            progress.SetId(checker.LastId("item_progress", "itp_id") + 1);

            // Insere os dados na tabela de itens
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@itp_id", progress.itp_id);

            cmd.ExecuteNonQuery();

            return progress.GetId();
        }

        /// <summary>: atualiza o progresso pelo id informado!
        /// <response>: retorna o id do progresso que sofreu a alteração.
        public int Update(int item_progress_id, ItemProgress progress)
        {
            cmd = new NpgsqlCommand(update, conn, tran);

            // Informa o id da requisição que será atualizada
            cmd.Parameters.AddWithValue("@itp_id", item_progress_id);

            // Registros que serão atualizados
            cmd.Parameters.AddWithValue("@itp_description", progress.itp_description);
            cmd.ExecuteNonQuery();

            return item_progress_id;
        }

        /// <summary>: o progresso é excluido permanentemente do registro!
        /// <response>: id do progresso que foi excluido.
        public int Delete(int item_progress_id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", item_progress_id);
            cmd.ExecuteNonQuery();

            return item_progress_id;
        }
    }
}