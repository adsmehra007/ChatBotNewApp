using System;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace ChatBot.Data
{
    public class DataAccess
    {
        public DataSet executeDB2Statement(string sqlStatement)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (OleDbConnection dbConnection = new OleDbConnection())
                {
                    dbConnection.ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                    dbConnection.Open();
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(sqlStatement, dbConnection))
                    {
                        dataAdapter.Fill(dataSet);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                //log here
            }
            return dataSet;
        }
        public DataSet executeSqlStatement(string sqlStatement)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection dbConnection = new SqlConnection())
                {
                    dbConnection.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
                    dbConnection.Open();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlStatement, dbConnection))
                    {
                        dataAdapter.Fill(dataSet);
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                //log here
            }
            return dataSet;
        }

        public int executeInsertSqlStatement(string sqlStatement)
        {
            int status = 0;
            try
            {
                using (SqlConnection dbConnection = new SqlConnection())
                {
                    dbConnection.ConnectionString = ConfigurationManager.AppSettings["SQLConnectionString"];
                    SqlCommand CmdObj = new SqlCommand(sqlStatement, dbConnection);
                    dbConnection.Open();
                    status = CmdObj.ExecuteNonQuery();
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
            }
            return status;
        }
    }
}