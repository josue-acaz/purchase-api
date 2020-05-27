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
    public class ItemProgressService
    {
        private Database db = new Database();
        private NpgsqlConnection connection = new NpgsqlConnection();

        /// <summary>Retorna um determinado item</sumary>
        public ObjResult GetById(int item_progress_id)
        {
            connection = db.GetCon();
            connection.Open();

            ItemProgress progress = new ItemProgress();
            ObjResult result = new ObjResult();
            ItemProgressDao dao = new ItemProgressDao(connection, null);

            try
            {
                progress = dao.GetById(item_progress_id);
                result.SetData(progress);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Cria um novo item e atribui à uma requisição</sumary>
        public ObjResult Post(ItemProgress progress, int user_id, int item_id, int item_status_id)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            ItemProgressDao dao = new ItemProgressDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                progress.SetId(dao.Post(progress, user_id, item_id, item_status_id));
                transaction.Commit();
                result.Success();
                result.SetData(progress.GetId());
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Atualiza um item</sumary>
        public ObjResult Update(int item_progress_id, ItemProgress progress)
        {
            connection = db.GetCon();
            connection.Open();

            NpgsqlTransaction transaction = connection.BeginTransaction();
            ItemProgressDao dao = new ItemProgressDao(connection, transaction);
            ObjResult result = new ObjResult();

            try
            {
                item_progress_id = dao.Update(item_progress_id, progress);
                transaction.Commit();
                result.Success();
                result.SetData(item_progress_id);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception(e.ToString());
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

        /// <summary>Exclui um determinado progresso de item</sumary>
        public ObjResult Delete(int item_progress_id)
        {
            connection = db.GetCon();
            connection.Open();

            ItemProgressDao dao = new ItemProgressDao(connection, null);
            ObjResult result = new ObjResult();

            try
            {
                item_progress_id = dao.Delete(item_progress_id);
                result.SetData(item_progress_id);
                result.Success();
            }
            finally
            {
                connection.Close();
                db.Close();
            }

            return result;
        }

    }
}