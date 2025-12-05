using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vossence.DATA.ORM
{
    public class DapperProcess : IDapper
    {
        private readonly IConfiguration config;

        #region Ctor
        public DapperProcess(IConfiguration config)
        {
            this.config = config;
        }
        #endregion

        #region Sql Connection
        public SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(config.GetConnectionString("MainConnection"));
            connection.Open();
            return connection;
        }
        #endregion

        #region Query
        public List<T> QueryApp<T>(string sql)
        {
            using IDbConnection db = GetOpenConnection();
            return db.Query<T>(sql).ToList();
        }
        #endregion

        #region Get 
        public T Get<T>(string sp, DynamicParameters parameters, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = GetOpenConnection();
            return db.Query<T>(sp, parameters, commandType: commandType).FirstOrDefault()!;
        }
        #endregion

        #region List    
        public List<T> GetAll<T>(string sp, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = GetOpenConnection();
            return db.Query<T>(sp, parameters, commandType: commandType).ToList();
        }
        #endregion

        #region Create
        public T Insert<T>(string sp, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = GetOpenConnection();
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parameters, commandType: commandType, transaction: tran).FirstOrDefault()!;
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }
        #endregion

        #region Update
        public T Update<T>(string sp, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = GetOpenConnection();
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parameters, commandType: commandType, transaction: tran).FirstOrDefault()!;
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
            return result;
        }

        #endregion

        #region Delete
        public T Delete<T>(string sp, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = GetOpenConnection();
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parameters, commandType: commandType, transaction: tran).FirstOrDefault()!;
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }
            return result;
        }

        #endregion

        #region Dispose
        public void Dispose()
        {

        }
        #endregion

    }

}
