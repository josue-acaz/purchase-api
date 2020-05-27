using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Helpers;
using Serilog;
using Newtonsoft.Json.Linq;
using Voartec.Cryptography;

namespace Voartec.Dao
{
    public class UserDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;
        private Crypto crypto = new Crypto();

        // Querys
        private string getById = "select * from _User where use_id=@use_id;";
        private string post = "insert into _User(use_id, use_ent_id, use_name, use_code, use_email, use_sector, use_function, use_phone, use_password, use_image, use_active, use_excluded) values(@use_id, @use_ent_id, @use_name, @use_code, @use_email, @use_sector, @use_function, @use_phone, @use_password, @use_image, @use_active, @use_excluded);";
        private string update = "update _User set use_ent_id=@use_ent_id, use_name=@use_name, use_code=@use_code, use_email=@use_email, use_sector=@use_sector, use_function=@use_function, use_phone=@use_phone, use_active=@use_active;";
        private string delete = "update _user set use_excluded=1 where use_id=@use_id";
        private string getPermission = "select * from user_permission left join resource on(res_id = per_resource_id) where per_user_id=@per_user_id and res_key_word=@res_key_word;";
        private string userConfig = "select * from user_config where usc_user_id=@usc_user_id;";
        private string searchUser = "select * from _user where (use_name=@use_name or use_email=@use_email) and use_excluded=0;";
        private string changePassword = "update _user set use_password=@use_password where use_id=@use_id";
        private string usedKeygen = "update reset_password set res_used=1 where res_user_id=@use_id";
        private string validPassword = "select * from _user where use_id=@use_id and use_password=@use_password";
        private string saveProfile = "update _user set use_name=@use_name, use_code=@use_code, use_email=@use_email, use_full_name=@use_full_name where use_id=@use_id";
        private string getProfile = "select * from _user where use_id=@use_id and use_excluded=0;";
        private string resetPassword = "insert into reset_password(res_key, res_user_id, res_used) values(@res_key, @res_user_id, @res_used)";
        private string validKeygen = "select * from reset_password where res_key=@res_key and res_used=0";
        private string authenticate = "select * from _user left join entity on ent_id=use_ent_id where (use_email=@use_email  or use_name=@use_name) and use_password=@use_password and use_active=true and use_excluded=false;";
        private string system_configuration = "select * from system_configuration";

        //Listagem
        private string list = "select * from _user where use_excluded=false";
        private string records = "count(use_id)";
        private string pagination = "limit @row_per_page offset(@current_page - 1) * @row_per_page;";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public UserDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: obtém um determinado usuário!
        /// <response>: os dados do usuário.
        public User GetById(int user_id)
        {
            User user = new User();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@use_id", user_id);
            reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                user.use_id = (int)reader["use_id"];
                user.use_ent_id = (int)reader["use_ent_id"];
                user.use_name = (string)reader["use_name"];
                user.use_code = (string)reader["use_code"];
                user.use_email = (string)reader["use_email"];
                user.use_sector = (string)reader["use_sector"];
                user.use_function = (string)reader["use_function"];
                user.use_phone = (string)reader["use_phone"];
                user.use_password = ""; //(string)reader["use_password"]
                user.use_image = (string)reader["use_image"];
                user.use_active = (bool)reader["use_active"];
                user.use_excluded = (bool)reader["use_excluded"];
            }

            reader.Close();
            return user;
        }

        /// <summary>: cria um novo usuário
        /// <response>: o id do usuário que foi criado.
        public int Post(User user)
        {
            // Atribue o id do usuário
            SchemeChecker checker = new SchemeChecker(conn, tran);
            user.SetId(checker.LastId("_user", "use_id") + 1);

            // Insere os dados na tabela de usuários
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", user.use_id);
            cmd.Parameters.AddWithValue("@use_ent_id", user.use_ent_id);
            cmd.Parameters.AddWithValue("@use_name", user.use_name);
            cmd.Parameters.AddWithValue("@use_code", user.use_code);
            cmd.Parameters.AddWithValue("@use_email", user.use_email);
            cmd.Parameters.AddWithValue("@use_sector", user.use_sector);
            cmd.Parameters.AddWithValue("@use_function", user.use_function);
            cmd.Parameters.AddWithValue("@use_phone", user.use_phone);
            cmd.Parameters.AddWithValue("@use_password", Crypto.HashMD5(user.use_password));
            cmd.Parameters.AddWithValue("@use_image", user.use_image);
            cmd.Parameters.AddWithValue("@use_active", user.use_active);
            cmd.Parameters.AddWithValue("@use_excluded", user.use_excluded);
            cmd.ExecuteNonQuery();

            return user.GetId();
        }

        /// <summary>: atualiza um usuário pelo id informado!
        /// <response>: retorna o id do usuário atualizado.
        public int Update(int user_id, User user)
        {
            cmd = new NpgsqlCommand(update, conn, tran);

            // Informa o id do usuário que será atualizado
            cmd.Parameters.AddWithValue("@use_id", user_id);

            // Registros que serão atualizados
            cmd.Parameters.AddWithValue("@use_ent_id", user.use_ent_id);
            cmd.Parameters.AddWithValue("@use_name", user.use_name);
            cmd.Parameters.AddWithValue("@use_code", user.use_code);
            cmd.Parameters.AddWithValue("@use_email", user.use_email);
            cmd.Parameters.AddWithValue("@use_sector", user.use_sector);
            cmd.Parameters.AddWithValue("@use_function", user.use_function);
            cmd.Parameters.AddWithValue("@use_phone", user.use_phone);
            cmd.Parameters.AddWithValue("@use_active", user.use_active);
            
            cmd.ExecuteNonQuery();

            return user_id;
        }

        /// <summary>: exclui um determinado usuário do sistema!
        public int Delete(int id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", id);
            cmd.ExecuteNonQuery();

            return id;
        }

        /// <summary>: Verifica se o usuário atual possui autorização para realizar a ação!
        /// <response>: retorna false se o usuário não possui permissão, true caso contrário.
        public bool GetPermission(int user_id, string resource, string action)
        {
            bool ret = false;

            cmd = new NpgsqlCommand(getPermission, conn, tran);
            cmd.Parameters.AddWithValue("@per_user_id", user_id);
            cmd.Parameters.AddWithValue("@res_key_word", resource);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (action == "create") { ret = (bool)reader["per_create"].Equals(true); }
                if (action == "read") { ret = (bool)reader["per_read"].Equals(true); }
                if (action == "update") { ret = (bool)reader["per_update"].Equals(true); }
                if (action == "delete") { ret = (bool)reader["per_delete"].Equals(true); }
            }
            reader.Close();
            return ret;
        }

        /// <summary>
        /// Retorna as configurações de um determinado usuário do sistema
        /// </summary>
        /// <param name="usc_user_id">É o id do usuário</param>
        /// <returns>Lista de configurações do usuário</returns>
        public List<UserConfig> UserConfig(int usc_user_id)
        {
            List<UserConfig> userConfigs = new List<UserConfig>();
            cmd = new NpgsqlCommand(userConfig, conn);
            cmd.Parameters.AddWithValue("@usc_user_id", usc_user_id);

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UserConfig userConfig = new UserConfig();
                userConfig.usc_user_id = (int)reader["usc_user_id"];
                userConfig.usc_config_name = reader["usc_config_name"].ToString();
                userConfig.usc_config_value = reader["usc_config_value"].ToString();
                userConfigs.Add(userConfig);
            }
            reader.Close();

            return userConfigs;
        }

        /// <summary>
        /// Faz a busca por um determinado usuário quando for buscado por meio de texto
        /// </summary>
        /// <param name="text_search">É o texto que será usado para pesquisar o usuário</param>
        /// <returns>Os dados do usuário encontrado</returns>
        public User SearchUser(string text_search)
        {
            User user = new User();

            cmd = new NpgsqlCommand(searchUser, conn);
            cmd.Parameters.AddWithValue("@use_name", text_search);
            cmd.Parameters.AddWithValue("@use_email", text_search);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                user.use_id = (int)reader["use_id"];
                user.use_name = reader["use_name"].ToString();
                user.use_code = reader["use_code"].ToString();
                user.use_email = reader["use_email"].ToString();
                user.use_image = reader["use_image"].ToString();
                user.use_active = (bool)reader["use_active"];
                user.use_excluded = (bool)reader["use_excluded"];
            }
            reader.Close();

            return user;
        }

        /// <summary>: altera a senha de um determinado usuário!
        /// <response>: o id do usuário que teve a senha alterada.
        public int ChangePassword(int use_id, string use_password)
        {
            cmd = new NpgsqlCommand(changePassword, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", use_id);
            cmd.Parameters.AddWithValue("@use_password", Crypto.HashMD5(use_password));
            cmd.ExecuteNonQuery();

            return use_id;
        }

        /// <summary>: Verifica se a senha já fui utilizada pela usuário!
        public bool UsedKeygen(int use_id)
        {
            cmd = new NpgsqlCommand(usedKeygen, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", use_id);
            cmd.ExecuteNonQuery();

            return true;
        }

        /// <summary>: Verifica se a senha é válida!
        public bool ValidPassword(int use_id, string password)
        {
            cmd = new NpgsqlCommand(validPassword, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", use_id);
            cmd.Parameters.AddWithValue("@use_password", Crypto.HashMD5(password));

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        /// <verificar>: verificar relação do perfil do usuário e a tabela de usuários
        public int SaveProfile(UserProfile profile)
        {
            cmd = new NpgsqlCommand(saveProfile, conn, tran);
            cmd.Parameters.AddWithValue("@use_id", profile.use_id);
            cmd.Parameters.AddWithValue("@use_name", profile.use_name);
            cmd.Parameters.AddWithValue("@use_code", profile.use_code);
            cmd.Parameters.AddWithValue("@use_email", profile.use_email);
            cmd.Parameters.AddWithValue("@use_full_name", profile.use_full_name);
            cmd.ExecuteNonQuery();

            return profile.use_id;
        }

        /// <summary>: retorna o perfil de determinado usuário!
        public UserProfile GetProfile(int use_id)
        {
            UserProfile profile = new UserProfile();
            cmd = new NpgsqlCommand(getProfile, conn);
            cmd.Parameters.AddWithValue("@use_id", use_id);

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                profile.use_id = (int)reader["use_id"];
                profile.use_name = reader["use_name"].ToString();
                profile.use_code = reader["use_code"].ToString();
                profile.use_email = reader["use_email"].ToString();
                profile.use_full_name = reader["use_full_name"].ToString();
            }
            reader.Close();

            return profile;
        }

        /// <summary>: reseta a senha de determinado usuário referente à um recurso do sistema!
        public string ResetPassword(int use_id)
        {
            string keygen = crypto.GenerateKey();
            cmd = new NpgsqlCommand(resetPassword, conn, tran);
            cmd.Parameters.AddWithValue("@res_key", keygen);
            cmd.Parameters.AddWithValue("@res_user_id", use_id);
            cmd.Parameters.AddWithValue("@res_used", 0);
            cmd.ExecuteNonQuery();

            return keygen;
        }

        /// <summary>: verifica se a senha é válida para o recurso!
        public int ValidKeygen(string keygen)
        {
            int use_id = 0;

            cmd = new NpgsqlCommand(validKeygen, conn);
            cmd.Parameters.AddWithValue("@res_key", keygen);

            reader = cmd.ExecuteReader();
            if (reader.Read()) use_id = (int)reader["res_user_id"];
            reader.Close();

            return use_id;
        }

        /// <summary>: faz a autenticação do usuário!
        /// <response>: retorna os dados do usuário logado.
        public UserLogged Authenticate(string username, string password, string system_flag)
        {
            string hour_format = "";
            cmd = new NpgsqlCommand(system_configuration, conn, tran);
            reader = cmd.ExecuteReader();
            if (reader.Read()) hour_format = reader["con_hour_format"].ToString();
            reader.Close();

            UserLogged userLogged = new UserLogged();
            cmd = new NpgsqlCommand(authenticate, conn, tran);
            cmd.Parameters.AddWithValue("@use_name", username);
            cmd.Parameters.AddWithValue("@use_email", username);
            cmd.Parameters.AddWithValue("@use_code", username);
            cmd.Parameters.AddWithValue("@use_password", Crypto.HashMD5(password));

            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                userLogged.id = (int)reader["use_id"];
                userLogged.entity = (int)reader["use_ent_id"];
                userLogged.name = reader["use_name"].ToString();
                userLogged.email = reader["use_email"].ToString();
                userLogged.sector = reader["use_sector"].ToString();
                userLogged.function = reader["use_function"].ToString();
                userLogged.phone = reader["use_phone"].ToString();
                userLogged.entity_name = reader["ent_name"].ToString();
                userLogged.image = reader["use_image"].ToString();
                userLogged.hour_format = hour_format;
            }
            reader.Close();

            userLogged.configs = this.UserConfig(userLogged.id);
            return userLogged;
        }

        /// <summary>: faz a listagem de usuários!
        /// <response>: dados dos usuários pesquisados.
        public PaginationResult List(string param)
        {
            List<User> users = new List<User>();

            // Validação dos parâmetros de listagem
            dynamic data_param = JObject.Parse(param);
            if (data_param.order == null) data_param.order = "";
            string order_by = data_param.order == "" ? " use_id asc " : data_param.order;
            string text_search = data_param.text_search ?? "";
            bool list_inactive = data_param.list_inactive ?? false;
            int current_page = data_param.current_page ?? 1;
            int row_per_page = data_param.row_per_page ?? 10;
            int master_id = data_param.master_id ?? 0;

            List<ParamSql> listParam = new List<ParamSql>();
            string sql_order = " order by " + order_by;

            // Texto da pesquisa
            if (text_search != "")
            {
                this.list += " and use_name like @use_name ";
                listParam.Add(new ParamSql("@use_name", "%" + text_search + "%"));
            }

            // Listar inativos
            if (!list_inactive)
            {
                this.list += " and use_active=true ";
            }

            // Entidade à qual o usuário está vinculado
            if (master_id != 0)
            {
                this.list += " and use_entity = @master_id ";
                listParam.Add(new ParamSql("@master_id", master_id.ToString()));
            }

            //Console.WriteLine("User query --> " + list.Replace("*", records));

            // Contagem do número de registros
            cmd = new NpgsqlCommand(list.Replace("*", records), conn);
            foreach (ParamSql p in listParam)
            {
                cmd.Parameters.AddWithValue(p.name, p.value);
            }
            int TotalRecords = Convert.ToInt32(cmd.ExecuteScalar());
            int firstRegister = ((row_per_page * current_page) - row_per_page) + 1;
            int lastRegister = firstRegister + row_per_page - 1;

            // Paginação
            pagination = list + " " + sql_order + " " + pagination;
            cmd = new NpgsqlCommand(pagination, conn);
            foreach (ParamSql p in listParam)
            {
                cmd.Parameters.AddWithValue(p.name, p.value);
            }

            cmd.Parameters.AddWithValue("@row_per_page", row_per_page);
            cmd.Parameters.AddWithValue("@current_page", current_page);

            // Cálculo de páginas
            PaginationResult pages = new PaginationResult();
            int restoDiv = TotalRecords % row_per_page;
            int qtdePages = TotalRecords / row_per_page;
            if (restoDiv > 0) qtdePages++;
            pages.totalPages = qtdePages;
            pages.totalRecords = TotalRecords;

            Console.WriteLine("User query --> " + pagination);

            // Leitura dos dados
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                User user = new User();
                user.use_id = (int)reader["use_id"];
                user.use_ent_id = (int)reader["use_ent_id"];
                user.use_name = reader["use_name"].ToString();
                user.use_code = reader["use_code"].ToString();
                user.use_email = reader["use_email"].ToString();
                user.use_sector = reader["use_sector"].ToString();
                user.use_function = reader["use_function"].ToString();
                user.use_phone = reader["use_phone"].ToString();
                user.use_password = "";
                user.use_image = reader["use_image"].ToString();
                user.use_active = (bool)reader["use_active"];
                user.use_excluded = (bool)reader["use_excluded"];
                users.Add(user);
            }
            reader.Close();
            pages.resultStatus = "success";
            pages.data = new List<object>(users);

            return pages;
        }
    }
}
