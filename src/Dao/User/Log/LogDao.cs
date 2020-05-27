using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Helpers;

namespace Voartec.Dao
{
    public class LogDao
    {
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlCommand cmd;

        // Query
        private string post = "insert into system_log(id, userId, date, hour, resource, action, registerKey, registerCopy) values(@id, @userId, @date, @hour, @resource, @action, @registerKey, @registerCopy)";

        public LogDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>
        /// Cria um novo log
        /// </summary>
        /// <param name="log">É o objeto log que será criado</param>
        /// <returns>id do log</returns>
        public int Post(Log log)
        {
            SchemeChecker checker = new SchemeChecker(conn, tran);
            log.id = (checker.LastId("system_log", "id") + 1);

            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@id", log.id);
            cmd.Parameters.AddWithValue("@userId", log.userId);
            cmd.Parameters.AddWithValue("@date", log.date);
            cmd.Parameters.AddWithValue("@hour", log.hour);
            cmd.Parameters.AddWithValue("@resource", log.resource);
            cmd.Parameters.AddWithValue("@action", log.action);
            cmd.Parameters.AddWithValue("@registerKey", log.registerKey);
            cmd.Parameters.AddWithValue("@registerCopy", log.registerCopy);
            cmd.ExecuteNonQuery();

            return log.id;
        }
    }
}