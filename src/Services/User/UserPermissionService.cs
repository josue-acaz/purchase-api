using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Dao;
using Voartec.Business;
using Newtonsoft;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace Voartec.Services
{
    public class UserPermissionService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        public PaginationResult List(string param, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            PaginationResult objResult = new PaginationResult();
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            List<string> messages_list = new List<string>();
            UserPermissionDao dao = new UserPermissionDao(connection, null);

            try
            {
                messages_list = bus.Validate(null, user_id, action);
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }
                objResult = dao.List(param);
            }
            finally
            {
                connection.Close();
            }
            return objResult;
        }

        public ObjResult Get(int per_id, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            ObjResult objResult = new ObjResult();
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            UserPermission obj = new UserPermission();
            List<string> messages_list = new List<string>();
            UserPermissionDao dao = new UserPermissionDao(connection, null);

            //validações
            try
            {
                messages_list = bus.Validate(null, user_id, action);
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }

                obj = dao.GetById(per_id);
                if (obj.per_id != 0)
                {
                    objResult.resultStatus = "success";
                    objResult.data = obj;
                }
                else
                {
                    messages_list.Add("Registro não encontrado.");
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                }
            }
            finally
            {
                connection.Close();
            }

            return objResult;
        }

        public ObjResult Save(UserPermission obj, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();
            int id;

            ObjResult objResult = new ObjResult();
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            List<string> messages_list = new List<string>();

            //validações
            try
            {
                messages_list = bus.Validate(obj, user_id, action);
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }
            }
            finally
            {
                connection.Close();
            }

            NpgsqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                UserPermissionDao dao = new UserPermissionDao(connection, transaction);
                id = dao.Post(obj);

                LogDao logDao = new LogDao(connection, transaction);
                logDao.Post(new Log(user_id, "UserPermission", action, id.ToString(), JsonConvert.SerializeObject(obj)));

                objResult.resultStatus = "success";
                objResult.data = id;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return objResult;
        }

        public ObjResult SaveChecked(dynamic obj, int user_id, string action)
        {
            string data = Convert.ToString(obj);
            dynamic param = JObject.Parse(data);

            connection = db.GetCon();
            connection.Open();
            int id;

            ObjResult objResult = new ObjResult();
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            List<string> messages_list = new List<string>();
            
            //validações
            try
            {
                UserPermission obj2 = new UserPermission();
                obj2.per_id = param.per_id;

                messages_list = bus.Validate(obj2, user_id, action);
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }
            }
            finally
            {
                connection.Close();
            }

            NpgsqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                UserPermissionDao dao = new UserPermissionDao(connection, transaction);
                id = dao.SaveCheck(param);

                LogDao logDao = new LogDao(connection, transaction);
                logDao.Post(new Log(user_id, "UserPermission", action, id.ToString(), JsonConvert.SerializeObject(obj)));

                objResult.resultStatus = "success";
                objResult.data = id;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return objResult;
        }

        public ObjResult Delete(int id, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            ObjResult objResult = new ObjResult();
            UserPermission obj = new UserPermission();
            UserPermissionDao dao = new UserPermissionDao(connection, null);
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            List<string> messages_list = new List<string>();

            //validações
            try
            {
                obj = dao.GetById(id);
                messages_list = bus.Validate(obj, user_id, action);
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }
            }
            finally
            {
                connection.Close();
            }

            NpgsqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                id = new UserPermissionDao(connection, transaction).Delete(id);
                LogDao logDao = new LogDao(connection, transaction);
                logDao.Post(new Log(user_id, "UserPermission", action, id.ToString(), JsonConvert.SerializeObject(obj)));
                objResult.resultStatus = "success";
                objResult.data = id;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return objResult;
        }

        public ObjResult ApplyPerfil(dynamic obj, int user_id, string action)
        {
            connection = db.GetCon();
            connection.Open();

            ObjResult objResult = new ObjResult();
            UserPermissionBusiness bus = new UserPermissionBusiness(connection);
            List<string> resources = new List<string>();
            List<string> messages_list = new List<string>();
            int use_id = obj.use_id;

            //validações
            try
            {
                connection.Open();    
                UserPermission obj2 = new UserPermission();
                obj2.per_user_id = obj.use_id;

                messages_list = bus.Validate(obj2, user_id, "create");
                if (messages_list.Count > 0)
                {
                    objResult.resultStatus = "error";
                    objResult.resultMessages = messages_list;
                    connection.Close();
                    return objResult;
                }
            }
            finally
            {
                connection.Close();
            }

            NpgsqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                UserPermissionDao dao = new UserPermissionDao(connection, transaction);
                resources = dao.ListResources();
                for (int i = 0; i < resources.Count; i++)
                {
                    if (!dao.PermissionExists(use_id, Convert.ToInt32(resources[i])))
                    {
                        
                        UserPermission new_obj = new UserPermission();
                        new_obj.per_resource_id = Convert.ToInt32(resources[i]);
                        new_obj.per_user_id = use_id;

                        int id = dao.Post(new_obj);
                        LogDao logDao = new LogDao(connection, transaction);
                        logDao.Post(new Log(user_id, "UserPermission", "create", id.ToString(), JsonConvert.SerializeObject(obj)));
                        
                    }
                }

                objResult.resultStatus = "success";
                objResult.data = "ok";
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return objResult;
        }
    }
}