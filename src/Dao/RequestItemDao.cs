using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Services;
using Serilog;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;

namespace Voartec.Dao
{
    public class RequestItemDao
    {
        private NpgsqlCommand cmd;
        private NpgsqlConnection conn;
        private NpgsqlTransaction tran;
        private NpgsqlDataReader reader;

        // Querys
        private string getById = "select * from request_item where itm_id=@itm_id and itm_excluded=false;";
        private string post = "insert into request_item(itm_id, itm_request_id, itm_status_id, itm_pn, itm_quantity, itm_approved_quantity, itm_description, itm_application, itm_priority, itm_deadline, itm_active, itm_excluded) values(@itm_id, @itm_request_id, @itm_status_id, @itm_pn, @itm_quantity, @itm_approved_quantity, @itm_description, @itm_application, @itm_priority, @itm_deadline, @itm_active, @itm_excluded);";
        private string delete = "update request_item set itm_excluded=true where itm_id=@itm_id;";
        private string update = "update request_item set itm_quantity=@itm_quantity, itm_approved_quantity=@itm_approved_quantity, itm_description=@itm_description, itm_application=@itm_application where itm_id=@itm_id;";
        private string cancel = "update request_item set itm_active=false where itm_id=@itm_id;";
        private string setStatus = "update request_item set itm_status_id=@itm_status_id where itm_id=@itm_id;";
        private string getRequestDependencies = "select req_id, req_application, req_priority, req_deadline from request where req_id=@req_id;";
        private string getItemsByRequest = "select * from request_item where itm_request_id=@itm_request_id and itm_excluded=false;";
        private string getRequestByItem = "";
        private string setApprovedQuantity = "update request_item set itm_approved_quantity=@itm_approved_quantity where itm_id=@itm_id;";

        private string list = "select * from request_item item where item.itm_excluded=false";
        private string pagination = "limit @row_per_page offset(@current_page - 1) * @row_per_page;";
        private string records = "count(itm_id) as amount";

        // Método construtor que recebe a conexão e transação atuais com o banco de dados
        public RequestItemDao(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            conn = connection;
            tran = transaction;
        }

        /// <summary>: obtém uma determinado item!
        /// <response>: os dados do item.
        public RequestItem GetById(int request_item_id)
        {
            RequestItem item = new RequestItem();
            cmd = new NpgsqlCommand(getById, conn);
            cmd.Parameters.AddWithValue("@itm_id", request_item_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                item.itm_id = (int)reader["itm_id"];
                item.itm_request_id = (int)reader["itm_request_id"];
                item.itm_pn = (string)reader["itm_pn"];
                item.itm_quantity = (int)reader["itm_quantity"];
                item.itm_approved_quantity = (int)reader["itm_approved_quantity"];
                item.itm_description = (string)reader["itm_description"];
                item.itm_application = (string)reader["itm_application"];
                item.itm_priority = (string)reader["itm_priority"];
                item.itm_deadline = (DateTime)reader["itm_deadline"];
                item.itm_active = (bool)reader["itm_active"];
                item.itm_excluded = (bool)reader["itm_excluded"];
            }
            reader.Close();

            return item;
        }

        /// <summary>: atualiza uma item pelo id informado!
        /// <response>: retorna o id do item que sofreu a alteração.
        public int Update(int item_id, RequestItem item)
        {
            cmd = new NpgsqlCommand(update, conn, tran);

            // Informa o id da requisição que será atualizada
            cmd.Parameters.AddWithValue("@itm_id", item_id);

            // Fornecer status do item
            // cmd.Parameters.AddWithValue("@itm_status_id", item.itm_status_id);
            cmd.Parameters.AddWithValue("@itm_quantity", item.itm_quantity);
            // Fornecer a quantidade aprovada
            cmd.Parameters.AddWithValue("@itm_approved_quantity", item.itm_approved_quantity);
            cmd.Parameters.AddWithValue("@itm_description", item.itm_description);
            cmd.Parameters.AddWithValue("@itm_application", item.itm_application);
            cmd.ExecuteNonQuery();

            return item_id;
        }

        /// <summary>: o item é marcado como excluido, o registro permanece!
        /// <response>: id do item que foi excluido.
        public int Delete(int request_item_id)
        {
            cmd = new NpgsqlCommand(delete, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", request_item_id);
            cmd.ExecuteNonQuery();

            return request_item_id;
        }

        /// <summary>: cria um novo item e o associa à uma requisição!
        /// <response>: retorna o id do item criado.
        public int Post(RequestItem item)
        {
            // Herda a aplicação e prioridade da requisição
            RequestService req_service = new RequestService();
            item.SetApplication(req_service.GetApplication(item.GetRequestId()));
            //item.SetPriority(req_service.GetPriority(item.GetRequestId()));

            // Atribue o id do item
            SchemeChecker checker = new SchemeChecker(conn, tran);
            item.SetId(checker.LastId("request_item", "itm_id") + 1);

            // Verifica se a prioridade é a mesma da requisição

            // Insere os dados na tabela de itens
            cmd = new NpgsqlCommand(post, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", item.itm_id);
            cmd.Parameters.AddWithValue("@itm_request_id", item.itm_request_id);
            cmd.Parameters.AddWithValue("@itm_status_id", item.itm_status_id);
            cmd.Parameters.AddWithValue("@itm_pn", item.itm_pn);
            cmd.Parameters.AddWithValue("@itm_quantity", item.itm_quantity);
            // Por padrão, a quantidade aprovada recebe o mesmo valor da quantidade solicitada!
            cmd.Parameters.AddWithValue("@itm_approved_quantity", item.itm_quantity);
            cmd.Parameters.AddWithValue("@itm_description", item.itm_description);
            cmd.Parameters.AddWithValue("@itm_application", item.GetApplication());
            cmd.Parameters.AddWithValue("@itm_priority", item.itm_priority);
            cmd.Parameters.AddWithValue("@itm_deadline", item.itm_deadline);
            cmd.Parameters.AddWithValue("@itm_active", item.itm_active);
            cmd.Parameters.AddWithValue("@itm_excluded", item.itm_excluded);

            cmd.ExecuteNonQuery();

            return item.GetId();
        }

        /// <summary>: cancela um item!
        /// <response>: o id do item cancelado.
        public int Cancel(int item_id)
        {
            cmd = new NpgsqlCommand(cancel, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", item_id);
            cmd.ExecuteNonQuery();

            return item_id;
        }

        /// <summary>: define um determinado status para o item!
        /// <response>: o id do item que teve o status alterado.
        public int SetStatus(int item_id, string status_name)
        {
            cmd = new NpgsqlCommand(setStatus, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", item_id);

            // Acessar serviço de status
            ItemStatusService service = new ItemStatusService();
            cmd.Parameters.AddWithValue("@itm_status_id", service.GetStatusIdByName(status_name));
            cmd.ExecuteNonQuery();

            return item_id;
        }

        /// <summary>: define a quantidade aprovada para determinado item!
        /// <response>: o id do item que teve a quantidade alterada.
        public int SetApprovedQuantity(int item_id, int approved_quantity)
        {
            cmd = new NpgsqlCommand(setApprovedQuantity, conn, tran);
            cmd.Parameters.AddWithValue("@itm_id", item_id);
            cmd.Parameters.AddWithValue("@itm_approved_quantity", approved_quantity);
            cmd.ExecuteNonQuery();

            return item_id;
        }

        /// <sumary>: lista os itens pelos filtros informados!
        /// <response>: os itens que contém os dados filtrados.
        public PaginationResult List(string param)
        {
            dynamic data = JObject.Parse(param);
            string filterByRequestId = " and item.itm_request_id=@itm_request_id";
            Console.WriteLine("Pesquisa --> " + data);

            List<RequestItem> items = new List<RequestItem>();
            PaginationResult pages = new PaginationResult();

            string text_search = data.text_search ?? "";
            bool list_inactive = data.list_inactive ?? false;
            int current_page = data.current_page ?? 1;
            int row_per_page = data.row_per_page ?? 10;
            int itm_request_id = data.itm_request_id ?? 0;
            
            // Verifica se é para mostrar os inativos
            if(!list_inactive)
            {
                list += " and item.itm_active=true";
            }

            // Adiciona os filtros por prioridade e descrição
            if(text_search != "")
            {
                string filterByPn = "itm_pn like '%" + text_search + "%'";
                string filterByDescription = "itm_description like '%" + text_search + "%'";
                //string filterByRequestId = "itm_request_id=" + text_search;
                //list += " and(" + filterByPn + " or " + filterByDescription + " or " + filterByRequestId + ");";
                list += " and(" + filterByPn + " or " + filterByDescription + ")";
            }

            // Contar a quantidade de registros.
            cmd = new NpgsqlCommand(list.Replace("*", records), conn);
            int total_records = Convert.ToInt32(cmd.ExecuteScalar());

            if(itm_request_id != 0)
            {
                pagination = list + filterByRequestId + " " + pagination;
            }
            else{
                pagination = list + " " + pagination;
            }

            Console.WriteLine("Query --> " + pagination);
            cmd = new NpgsqlCommand(pagination, conn);
            cmd.Parameters.AddWithValue("@row_per_page", row_per_page);
            cmd.Parameters.AddWithValue("@current_page", current_page);

            Console.WriteLine("Id da requisição --> " + itm_request_id);
            if(itm_request_id != 0)
            {
                cmd.Parameters.AddWithValue("@itm_request_id", itm_request_id);
            }

            int rest_division = total_records % row_per_page;
            int amount_pages = total_records / row_per_page;

            if(rest_division > 0) { amount_pages++; }
            pages.totalPages = amount_pages;
            pages.totalRecords = total_records;

            reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                RequestItem item = new RequestItem();
                item.itm_id = (int)reader["itm_id"];
                item.itm_request_id = (int)reader["itm_request_id"];
                item.itm_status_id = (int)reader["itm_status_id"];
                item.itm_pn = (string)reader["itm_pn"];
                item.itm_quantity = (int)reader["itm_quantity"];
                item.itm_approved_quantity = (int)reader["itm_approved_quantity"];
                item.itm_description = (string)reader["itm_description"];
                item.itm_application = (string)reader["itm_application"];
                item.itm_priority = (string)reader["itm_priority"];
                item.itm_deadline = (DateTime)reader["itm_deadline"];
                item.itm_active = (bool)reader["itm_active"];
                item.itm_excluded = (bool)reader["itm_excluded"];
                items.Add(item);
            }

            pages.resultStatus = "success";
            pages.data = new List<object>(items);

            return pages;
        }

        /// <sumary>: obtém as dependencias da requisição!
        /// <response>: a prioridade, data limite e aplicação da requisição são retornados.
        public Request GetRequestDependencies(int request_id)
        {
            Request request = new Request();
            cmd = new NpgsqlCommand(getRequestDependencies, conn);
            cmd.Parameters.AddWithValue("@req_id", request_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                request.req_id = (int)reader["req_id"];
                request.req_application = (string)reader["req_application"];
                request.req_priority = (string)reader["req_priority"];
                request.req_deadline = (DateTime)reader["req_deadline"];
            }
            reader.Close();

            return request;
        }

        /// <sumary>: obtém os itens de uma requisição!
        /// <response>: os itens de uma requisição.
        public List<RequestItem> GetItemsByRequest(int request_id)
        {
            List<RequestItem> items = new List<RequestItem>();
            cmd = new NpgsqlCommand(getItemsByRequest, conn);
            cmd.Parameters.AddWithValue("@itm_request_id", request_id);
            reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                RequestItem item = new RequestItem();
                item.itm_id = (int)reader["itm_id"];
                item.itm_request_id = (int)reader["itm_request_id"];
                item.itm_status_id = (int)reader["itm_status_id"];
                item.itm_pn = (string)reader["itm_pn"];
                item.itm_quantity = (int)reader["itm_quantity"];
                item.itm_approved_quantity = (int)reader["itm_approved_quantity"];
                item.itm_description = (string)reader["itm_description"];
                item.itm_application = (string)reader["itm_application"];
                item.itm_priority = (string)reader["itm_priority"];
                item.itm_deadline = (DateTime)reader["itm_deadline"];
                item.itm_active = (bool)reader["itm_active"];
                item.itm_excluded = (bool)reader["itm_excluded"];
                items.Add(item);
            }
            reader.Close();

            return items;
        }

        /// <sumary>: obtém uma determinada requisição que está relacionada à um determinado item!
        /// <response>: os dados da requisição.
        public Request GetRequestByItem(int item_id)
        {
            Request request = new Request();
            cmd = new NpgsqlCommand(getRequestByItem, conn);
            cmd.Parameters.AddWithValue("@itm_id", item_id);
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                request.req_id = (int)reader["req_id"];
                request.req_user_id = (int)reader["req_user_id"];
                request.req_sent_date_hour = (DateTime)reader["req_sent_date_hour"];
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

        // Verifica se todos os itens da requisição já passaram por aprovação
        public bool TechnicalApprovalIsFinished(int request_id)
        {
            bool isFinished = false;

            foreach (RequestItem item in GetItemsByRequest(request_id))
            {
                Console.WriteLine("Status dos itens --> " + item.itm_status_id);
                // Se o status é diferente de ED e AAT
                if(item.itm_status_id != 1 && item.itm_status_id != 2)
                {
                    isFinished = true;
                }
                else{
                    return false;
                }
            }

            return isFinished;
        }

        // Verifica se a aprovação foi completa
        public bool FullApproved(int request_id)
        {
            bool fullApproved = false;

            foreach (RequestItem item in GetItemsByRequest(request_id))
            {
                // Se o status do item é ATC e a quantidade solicitada é a mesma da quantidade aprovada
                if(item.itm_status_id == 4 && (item.itm_quantity == item.itm_approved_quantity))
                {
                    fullApproved = true;
                }
                else{
                    return false;
                }
            }

            return fullApproved;
        }

        // Verifica se a aprovação foi parcial
        public bool PartiallyApproved(int request_id)
        {
            bool partiallyApproved = false;

            foreach (RequestItem item in GetItemsByRequest(request_id))
            {
                // Se o status é ATN ou ATC ou a quantidade de itens solicitados é diferente da quantidade aprovada
                if(item.itm_status_id == 4 && (item.itm_quantity != item.itm_approved_quantity) || item.itm_status_id == 4 && (item.itm_quantity == item.itm_approved_quantity))
                {
                    partiallyApproved = true;
                }
            }

            return partiallyApproved;
        }

        // Verifica se não foi aprovada a requisição
        public bool NotApproved(int request_id)
        {
            bool notApproved = false;

            foreach (RequestItem item in GetItemsByRequest(request_id))
            {
                // Se todos os status são 'ATN'
                if(item.itm_status_id == 3)
                {
                    notApproved = true;
                }
                else{
                    return false;
                }
            }

            return notApproved;
        }
    }
}
