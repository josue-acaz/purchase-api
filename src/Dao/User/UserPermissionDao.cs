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
    public class UserPermissionDao
    {
        private NpgsqlConnection conn;
        private NpgsqlCommand cmd;
        private NpgsqlDataReader reader;
        private NpgsqlTransaction tran;

        // Querys
        private string getById = "select * from user_permission left join _user on (use_id=per_user_id) left join resource on (res_id=per_resource_id) where per_id<>0 and per_id = @per_id;";
        private string post = "insert into user_permission(per_id, per_resource_id, per_user_id, per_create, per_read, per_update, per_delete) values (@per_id, @per_resource_id, @per_user_id, @per_create, @per_read, @per_update, @per_delete);";
        private string delete = "delete user_permission where per_id=@per_id";
        private string update = "";
        private string permissionExists = "select * from user_permission where per_user_id=@per_user_id and per_resource_id=@per_resource_id";
        private string listResources = "select * from resource;";
        private string list = "select * from user_permission left join _user on (use_id=per_user_id) left join resource on (res_id=per_resource_id) where per_id<>0 ";

        public UserPermissionDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        public PaginationResult List(string param)
        {
            dynamic data_param = JObject.Parse(param);
            int master_id = data_param.master_id;

            if (data_param.master_id == null) data_param.master_id = "";
            if (data_param.resource_id == null) data_param.resource_id = "";

            int resource_id = data_param.resource_id == "" ? 0 : data_param.resource_id;

            List<ParamSql> listParam = new List<ParamSql>();
            PaginationResult pages = new PaginationResult();
            List<UserPermission> permissions = new List<UserPermission>();
            
            if (master_id != 0)
            {
                list += "and per_user_id = @per_user_id ";
                listParam.Add(new ParamSql("@per_user_id", master_id.ToString()));
            }
            if (resource_id != 0)
            {
                list += "and per_resource_id = @per_resource_id ";
                listParam.Add(new ParamSql("@per_resource_id", resource_id.ToString()));
            }
            
            list +=" order by res_name";
            
            cmd = new NpgsqlCommand(list, conn);
            foreach (ParamSql p in listParam)
            {
                cmd.Parameters.AddWithValue(p.name, Convert.ToInt32(p.value));
            }

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UserPermission permission = new UserPermission();
                permission.per_id = (int)reader["per_id"];
                permission.per_resource_id = (int)reader["per_resource_id"];
                permission.per_create = (bool)reader["per_create"];
                permission.per_read = (bool)reader["per_read"];
                permission.per_update = (bool)reader["per_update"];
                permission.per_delete = (bool)reader["per_delete"];
                permission.resource_key_word = reader["res_key_word"].ToString();
                permission.resource_name = reader["res_name"].ToString();

                permissions.Add(permission);
            }
            reader.Close();
            pages.resultStatus = "success";
            pages.data = new List<object>(permissions);

            return pages;
        }

        public UserPermission GetById(int per_id)
        {
            UserPermission permission = new UserPermission();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@per_id", per_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                // Permission
                permission.per_id = (int)reader["per_id"];
                permission.per_resource_id = (int)reader["per_resource_id"];
                permission.per_create = (bool)reader["per_create"];
                permission.per_read = (bool)reader["per_read"];
                permission.per_update = (bool)reader["per_update"];
                permission.per_delete = (bool)reader["per_delete"];
                // Resource
                permission.resource_key_word = reader["res_key_word"].ToString();
                permission.resource_name = reader["res_name"].ToString();
            }
            reader.Close();
            return permission;
        }

        public int Post(UserPermission permission)
        {
            // Atribue o id da permissão
            SchemeChecker checker = new SchemeChecker(conn, tran);
            permission.per_id = (checker.LastId("user_permission", "per_id") + 1);
            
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@per_id", permission.per_id);
            cmd.Parameters.AddWithValue("@per_resource_id", permission.per_resource_id);
            cmd.Parameters.AddWithValue("@per_user_id", permission.per_user_id);
            cmd.Parameters.AddWithValue("@per_create", permission.per_create);
            cmd.Parameters.AddWithValue("@per_read", permission.per_read);
            cmd.Parameters.AddWithValue("@per_update", permission.per_update);
            cmd.Parameters.AddWithValue("@per_delete", permission.per_delete);
            cmd.ExecuteNonQuery();

            return permission.per_id;
        }

        /// <summary>
        /// Atualiza uma permissão, nesse caso alterar o usuário da permissão não é necessário,
        /// no entanto, pode ser possível alterar o recurso que esse usuário tem acesso.
        /// </summary>
        /// <param name="per_id">É o id da permissão que será atualizada</param>
        /// <param name="permission">É o objeto permisssão que contém as alterações que serão feitas</param>
        /// <returns></returns>
        public int Update(int per_id, UserPermission permission)
        {
            cmd = new NpgsqlCommand(update, conn, tran);

            // Informa o id da permissão que será atualizada
            cmd.Parameters.AddWithValue("@per_id", permission.per_id);

            // Registros que serão atualizados
            cmd.Parameters.AddWithValue("@per_resource_id", permission.per_resource_id);
            cmd.Parameters.AddWithValue("@per_create", permission.per_create);
            cmd.Parameters.AddWithValue("@per_read", permission.per_read);
            cmd.Parameters.AddWithValue("@per_update", permission.per_update);
            cmd.Parameters.AddWithValue("@per_delete", permission.per_delete);
            
            cmd.ExecuteNonQuery();

            return per_id;
        }

        public int SaveCheck(dynamic obj)
        {
            int id = obj.per_id;
            bool is_checked = obj.is_checked;
            string field_update = "";
            if (obj.action == "read") field_update = "per_read";
            if (obj.action == "update") field_update = "per_update";
            if (obj.action == "create") field_update = "per_create";
            if (obj.action == "delete") field_update = "per_delete";

            cmd = new NpgsqlCommand("update user_permission set " + field_update + " = @field_update where per_id=@per_id ", conn, tran);
            cmd.Parameters.AddWithValue("@per_id", id);
            cmd.Parameters.AddWithValue("@field_update", is_checked);
            cmd.ExecuteNonQuery();
            return obj.per_id;
        }

        public int Delete(int per_id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@per_id", per_id);
            cmd.ExecuteNonQuery();

            return per_id;
        }

        public bool PermissionExists(int use_id, int res_id)
        {
            cmd = new NpgsqlCommand(permissionExists, conn, tran);
            cmd.Parameters.AddWithValue("@per_user_id", use_id);
            cmd.Parameters.AddWithValue("@per_resource_id", res_id);
            reader = cmd.ExecuteReader();
            bool exists = reader.Read();
            reader.Close();

            return exists;
        }

        public List<string> ListResources()
        {
            List<string> resources = new List<string>();
            cmd = new NpgsqlCommand(listResources, conn, tran);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                resources.Add(reader["res_id"].ToString());
            }
            reader.Close();
            return resources;
        }
    }
}