using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Helpers;
using Serilog;

namespace Voartec.Dao
{
    public class ItemStatusDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;

        // Querys
        private string getStatusNameById = "select its_id from item_status where its_name like @its_name and its_excluded=false;";
        private string currentStatus = "select item_status.its_id from item_status inner join request_item on(request_item.itm_status_id=item_status.its_id) where request_item.itm_id=@request_item.itm_id;";
        private string changeStatus = "update request_item set itm_status_id=@itm_status_id where itm_id=@itm_id;";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public ItemStatusDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: obtém o id de determinado status ativo!
        /// <response>: o id do status buscado.
        public int GetStatusIdByName(string status_name)
        {
            ItemStatus status = new ItemStatus();
            cmd = new NpgsqlCommand(getStatusNameById, conn);
            cmd.Parameters.AddWithValue("@its_name", status_name);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                status.SetId((int)reader["its_id"]);
            }
            reader.Close();

            return status.GetId();
        }

        /// <summary>: altera o status do item!
        /// <response>: o id do item modificado.
        public void ChangeStatus(int request_item_id, int new_status)
        {
            cmd = new NpgsqlCommand(changeStatus, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", request_item_id);
            cmd.Parameters.AddWithValue("@itm_status_id", new_status);
            cmd.ExecuteNonQuery();
        }

        /// <summary>: obtém o status atual do item!
        /// <response>: o id do status buscado.
        public int CurrentStatus(int request_item_id)
        {
            ItemStatus status = new ItemStatus();
            cmd = new NpgsqlCommand(currentStatus, conn);
            cmd.Parameters.AddWithValue("@request_item.itm_id", request_item_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                status.its_id = (int)reader["its_id"];
            }
            reader.Close();

            return status.GetId();
        }
    }
}
