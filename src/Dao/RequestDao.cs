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
    public class RequestDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;

        // Querys
        //private string get = "select * from request where req_excluded=false";
        private string update = "update request set req_description=@req_description, req_application=@req_application, req_priority=@req_priority, req_deadline=@req_deadline where req_id=@req_id;";
        private string post = "insert into request(req_id, req_user_id, req_sent_date_hour, req_description, req_application, req_priority, req_deadline, req_status, req_active, req_excluded) values (@req_id, @req_user_id, current_timestamp, @req_description, @req_application, @req_priority, @req_deadline, @req_status, @req_active, @req_excluded);";
        private string delete = "update request set req_excluded=true where req_id=@req_id;";
        private string getById = "select * from request where req_id=@req_id;";
        private string cancel = "update request set req_active=false where req_id=@req_id;";
        private string changeStatus = "update request set req_status=@req_status where req_id=@req_id;";
        private string application = "select req_application from request where req_id=@req_id;";
        private string priority = "select req_priority from request where req_id=@req_id;";

        /// <sumary>Listagem dos registros das requisições</sumary>
        /// <param decorator="@order">Se os registros serão ordenados do mais atual para o mais antigo e vice-versa</param>
        /// <param decorator="@row_per_page">Número de registros por página</param>
        /// <param decorator="@current_page">Página atual</param>
        private string list = "select * from request r where r.req_excluded=false";
        private string pagination = "limit @row_per_page offset(@current_page - 1) * @row_per_page;";
        private string records = "count(req_id) as amount";
        private string order = "order by req_sent_date_hour @order";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public RequestDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: obtém todas as requisições, sendo essas ativas ou não!
        /// <response>: retorna um array de json com todas as requisições.
        public void Get()
        {}

        /// <summary>: atualiza uma requisição pelo id informado!
        /// <response>: retorna o id da requisição atualizada.
        public int Update(int request_id, Request request)
        {
            cmd = new NpgsqlCommand(update, conn, tran);

            // Informa o id da requisição que será atualizada
            cmd.Parameters.AddWithValue("@req_id", request_id);

            // Registros que serão atualizados
            cmd.Parameters.AddWithValue("@req_description", request.req_description);
            cmd.Parameters.AddWithValue("@req_application", request.req_application);
            cmd.Parameters.AddWithValue("@req_priority", request.req_priority);
            cmd.Parameters.AddWithValue("@req_deadline", request.req_deadline);
            
            cmd.ExecuteNonQuery();

            return request_id;
        }

        /// <summary>: cria uma nova requisição!
        /// <reponse>: retorna o id da requisição criada.
        public int Post(Request request)
        {
            // Atribue o id da requisição
            SchemeChecker checker = new SchemeChecker(conn, tran);
            request.SetId(checker.LastId("request", "req_id") + 1);

            // Insere os dados na tabela de requisição
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@req_id", request.req_id);
            cmd.Parameters.AddWithValue("@req_user_id", request.req_user_id);
            cmd.Parameters.AddWithValue("@req_sent_date_hour", request.req_sent_date_hour);
            cmd.Parameters.AddWithValue("@req_description", request.req_description);
            cmd.Parameters.AddWithValue("@req_application", request.req_application);
            cmd.Parameters.AddWithValue("@req_priority", request.req_priority);
            cmd.Parameters.AddWithValue("@req_deadline", request.req_deadline);
            cmd.Parameters.AddWithValue("@req_status", request.req_status);
            cmd.Parameters.AddWithValue("@req_active", request.req_active);
            cmd.Parameters.AddWithValue("@req_excluded", request.req_excluded);
            cmd.ExecuteNonQuery();

            return request.GetId();
        }

        /// <summary>: a requisição é marcada como excluida, o registro permanece!
        /// <response>: id da requisição que foi excluída.
        public int Delete(int request_id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            cmd.ExecuteNonQuery();

            return request_id;
        }

        /// <summary>: obtém uma determinada requisição!
        /// <response>: os dados da requisição.
        public Request GetById(int request_id)
        {
            Request request = new Request();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                request.req_id = (int)reader["req_id"];
                request.req_user_id = (int)reader["req_user_id"];
                request.req_sent_date_hour = (DateTime?)reader["req_sent_date_hour"];
                request.req_description = (string)reader["req_description"];
                request.req_application = (string)reader["req_application"];
                request.req_priority = (string)reader["req_priority"];
                request.req_deadline = (DateTime)reader["req_deadline"];
                request.req_status = (string)reader["req_status"];
                request.req_active = (bool)reader["req_active"];
                request.req_excluded = (bool)reader["req_excluded"];
            }
            reader.Close();

            return request;
        }

        /// <sumary>: lista as requisições de acordo com os filtros informados!
        /// <response>: as requisições que contém os dados filtrados.
        public PaginationResult List(string param)
        {
            dynamic data = JObject.Parse(param);

            List<Request> requests = new List<Request>();
            PaginationResult pages = new PaginationResult();

            // Valores padrões casos os filtros sejam omitidos
            string order_by = data.order ?? "desc";
            string text_search = data.text_search ?? "";
            bool list_inactive = data.list_inactive ?? false;
            int current_page = data.current_page ?? 1;
            int row_per_page = data.row_per_page ?? 10;
            bool list_open = data.list_open ?? true;
			bool list_finished = data.list_finished ?? true;

            // Verifica se é para mostrar os inativos
            if(!list_inactive)
            {
                list += " and r.req_active=true";
            }

            // Verifica se é para mostrar as requisições abertas ou efetivadas
            if (!list_open) this.list += " and req_status != 'E' ";
			if (!list_finished) this.list += " and req_status != 'A' ";

            // Adicionar o tipo de ordenação de registros, se é pelo mais recente ou mais antigo
            order = order.Replace("@order", order_by);

            // Adiciona os filtros por prioridade e descrição
            if(text_search != "")
            {
                string filterByPriority = "req_priority like '%" + text_search + "%'";
                string filterByDescription = "req_description ilike '%" + text_search + "%'";
                list += " and(" + filterByPriority + " or " + filterByDescription + ")";
            }

            // Contar a quantidade de registros.
            cmd = new NpgsqlCommand(list.Replace("*", records), conn);
            int total_records = Convert.ToInt32(cmd.ExecuteScalar());

            pagination = list + " " + order + " " + pagination;
            cmd = new NpgsqlCommand(pagination, conn);
            cmd.Parameters.AddWithValue("@row_per_page", row_per_page);
            cmd.Parameters.AddWithValue("@current_page", current_page);

            int rest_division = total_records % row_per_page;
            int amount_pages = total_records / row_per_page;

            if(rest_division > 0) { amount_pages++; }
            pages.totalPages = amount_pages;
            pages.totalRecords = total_records;

            reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                Request request = new Request();
                request.req_id = (int)reader["req_id"];
                request.req_user_id = (int)reader["req_user_id"];
                request.req_sent_date_hour = (DateTime?)reader["req_sent_date_hour"];
                request.req_description = (string)reader["req_description"];
                request.req_application = (string)reader["req_application"];
                request.req_priority = (string)reader["req_priority"];
                request.req_deadline = (DateTime)reader["req_deadline"];
                request.req_status = (string)reader["req_status"];
                request.req_active = (bool)reader["req_active"];
                request.req_excluded = (bool)reader["req_excluded"];
                requests.Add(request);
            }

            reader.Close();
            pages.resultStatus = "success";
            pages.data = new List<object>(requests);

            return pages;
        }

        /// <summary>: cancela uma requisição!
        /// <response>: o id da requisição cancelada.
        public int Cancel(int request_id)
        {
            cmd = new NpgsqlCommand(cancel, conn, tran);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            cmd.ExecuteNonQuery();

            return request_id;
        }

        /// <summary>: muda o status de uma requisição!
        /// <response>: o id da requisição afetada.
        public int ChangeStatus(int request_id, char new_status)
        {
            cmd = new NpgsqlCommand(changeStatus, conn, tran);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            cmd.Parameters.AddWithValue("@req_status", new_status);
            cmd.ExecuteNonQuery();
            return request_id;
        }

        /// <summary>: obtém o nome da aplicação de uma determinada requisição
        /// <response>: o nome da aplicação da requisição.
        public string GetApplication(int request_id)
        {
            Request request = new Request();
            cmd = new NpgsqlCommand(application, conn);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                request.SetApplication((string)reader["req_application"]);
            }
            reader.Close();

            return request.GetApplication();
        }

        /// <summary>: obtém o nome a prioridade de uma determinada requisição
        /// <response>: o nome da prioridade da requisição.
        public string GetPriority(int request_id)
        {
            Request request = new Request();
            cmd = new NpgsqlCommand(priority, conn);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                request.SetPriority((string)reader["req_priority"]);
            }
            reader.Close();

            return request.GetPriority();
        }
    }
}
