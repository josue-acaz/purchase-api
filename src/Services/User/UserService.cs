using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Npgsql;
using Voartec.Dao;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Services;
using Voartec.Business;
using Voartec.Config;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Voartec.Services
{
    public class UserService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna um determinado usuário</sumary>
        public ObjResult GetById(int user_id)
        {
            connection = db.GetCon();
            connection.Open();

            User user = new User();
            ObjResult result = new ObjResult();
            UserDao dao = new UserDao(connection, null);

            try
            {
                user = dao.GetById(user_id);
                result.SetData(user);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Cria um novo usuário</sumary>
        public ObjResult Post(User user)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            UserDao dao = new UserDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                user.SetId(dao.Post(user));
                transaction.Commit();
                result.Success();
                result.SetData(user.GetId());
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>Faz a autenticação do usuário</sumary>
        public ObjResult Authenticate(string username, string password, string system_flag)
        {
            connection = db.GetCon();
            ObjResult objResult = new ObjResult();
            List<string> listMessages = new List<String>();

            try
            {
                connection.Open();
                
                LogDao logDao = new LogDao(connection, null);
                UserLogged user = new UserDao(connection, null).Authenticate(username, password, system_flag);

                // Se a autenticação for bem sucedida, gerar o token JWT
                if (user.id != 0)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var configuration = Builder.GetConfiguration();
                    var key = Encoding.ASCII.GetBytes(configuration.GetSection("Permissions:SecretKey").Value);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("use_id", user.id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    user.token = tokenHandler.WriteToken(token);

                    logDao.Post(new Log(user.id, "User", "Login", username, ""));
                    objResult.resultStatus = "success";
                    objResult.data = user;
                }
                else
                {
                    logDao.Post(new Log(0, "User", "ErroLogin", username, username + " : " + password));

                    listMessages.Add("Usuário ou senha inválidos.");
                    objResult.resultStatus = "error";
                    objResult.resultMessages = listMessages;
                }

            }
            catch (Exception e)
            {
                listMessages.Add(e.Message);
                objResult.resultStatus = "error";
                objResult.resultMessages = listMessages;
            }
            finally
            {
                connection.Close();
            }

            return objResult;
        }

        /// <summary>Lista todos os usuários do sistema</sumary>
        public PaginationResult List(string param, int user_id, string action)
        {
            connection = db.GetCon();
            PaginationResult result = new PaginationResult();

            try
            {
                connection.Open();
                UserBusiness bus = new UserBusiness(connection);
                List<string> messages_list = new List<string>();
                UserDao dao = new UserDao(connection, null);
                messages_list = bus.Validate(null, user_id, action);
                if (messages_list.Count > 0)
                {
                    result.resultStatus = "error";
                    result.resultMessages = messages_list;
                    connection.Close();
                    return result;
                }
                result = dao.List(param);
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        /// <summary>Altera a senha de acesso ao sistema de determinado usuário</sumary>
        public ObjResult ResetPassword(dynamic obj)
        {
            connection = db.GetCon();

            UserDao dao;
            User user = new User();
            ObjResult result = new ObjResult();
            List<string> messages_list = new List<string>();

            // validações
            try
            {
                connection.Open();
                dao = new UserDao(connection, null);
                string text_user = obj.user_reset;
                user = dao.SearchUser(text_user);

                if (user.use_id == 0) messages_list.Add("Usuário não encontrado.");
                if (user.use_email == "") messages_list.Add("Nenhum email cadastrado para esse usuário.");

                if (messages_list.Count > 0)
                {
                    result.resultStatus = "error";
                    result.resultMessages = messages_list;
                    connection.Close();
                    return result;
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

                dao = new UserDao(connection, transaction);
                string keygen = dao.ResetPassword(user.use_id);

                result.resultStatus = "success";
                result.data = user.use_email;

                var builder = Builder.GetConfiguration();
                //EmailHelper email = new EmailHelper();
                string texto = "<h2>Atenção.</h2> A recuperação de senha do seu usuário foi solicitado em nosso sistema. Utilize o link abaixo para criar uma nova senha.<br/><br/>";
                texto += "Click criar nova senha: " + builder.GetSection("AppSettings:url_reset_password").Value + "?key=" + keygen;
                //email.SendEmail(texto, user.use_email);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    if (transaction != null) transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("Message: {0}", ex2.Message);
                }
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return result;
        }
    }
}