using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;

namespace Voartec.Helpers
{
    public class SchemeChecker
    {
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlCommand cmd;

        public SchemeChecker(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: especificar o nome da tabela e o nome do id
        /// <response>: retorna o último id dos registros da tabela
        public int LastId(string table, string id)
        {
            cmd = new NpgsqlCommand("select coalesce(max("+id+"),0) from "+table, conn, tran);
            return (int)cmd.ExecuteScalar();
        }
    }
}