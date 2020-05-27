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
    public class ItemStatusService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna o id do status se o nome é informado</sumary>
        public int GetStatusIdByName(string status_name)
        {
            connection = db.GetCon();
            connection.Open();

            ItemStatus status = new ItemStatus();
            ItemStatusDao dao = new ItemStatusDao(connection, null);

            try
            {
                status.SetId(dao.GetStatusIdByName(status_name));
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return status.GetId();
        }

        /// <summary>Retorna o status atual do item</sumary>
        public int CurrentStatus(int request_item_id)
        {
            int item_status_id = 0;
            connection = db.GetCon();
            connection.Open();

            ItemStatus status = new ItemStatus();
            ItemStatusDao dao = new ItemStatusDao(connection, null);

            try
            {
                item_status_id = dao.CurrentStatus(request_item_id);
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return item_status_id;
        }

        /// <summary>Altera o status do item</sumary>
        public void ChangeStatus(int request_item_id, int new_status)
        {
            connection = db.GetCon();
            connection.Open();

            ItemStatusDao dao = new ItemStatusDao(connection, null);

            try
            {
                dao.ChangeStatus(request_item_id, new_status);
            }
            finally
            {
                connection.Close();
                db.Close();
            }
        }
    }
}