using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Maintenance.Infrastructure.SqlServer.Data
{
    public class DataContextSql : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _connectionStringHaiHaCo;
        private readonly IMapper _mapper;
        private SqlConnection _connection;
        private SqlConnection _connectionHaiHaCo;

        public enum SqlDbTarget
        {
            Default,
            HaiHaCo,
            Maintenance
        }

        public DataContextSql(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration.GetConnectionString("ApplicationDbContext") ?? "";
            _connectionStringHaiHaCo = configuration["Connections:Sql"] ?? "";
            _connection = new SqlConnection(_connectionString);
            _connectionHaiHaCo = new SqlConnection(_connectionStringHaiHaCo);
            _mapper = mapper;
        }

        private SqlConnection GetConnection(SqlDbTarget dbTarget)
        {
            return dbTarget switch
            {
                SqlDbTarget.HaiHaCo => _connectionHaiHaCo,
                _ => _connection,
            };
        }

        public void OpenConnection(SqlDbTarget dbTarget = SqlDbTarget.Default)
        {
            var connection = GetConnection(dbTarget);
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection(SqlDbTarget dbTarget = SqlDbTarget.Default)
        {
            var connection = GetConnection(dbTarget);
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public IEnumerable<T> ExecuteQuery<T>(string query, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        var results = new List<T>();
                        while (reader.Read())
                        {
                            results.Add((T)reader.GetValue(0));
                        }
                        return results;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing query [{query}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling query [{query}]: {ex.Message}", ex);
            }
        }

        public IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    var results = dataTable.AsEnumerable().Select(row => _mapper.Map<T>(row)).ToList();
                    return results;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing stored procedure [{procedureName}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling stored procedure [{procedureName}]: {ex.Message}", ex);
            }
        }

        public List<List<DataRow>> ExecuteStoredProcedureRawMultiple(string procedureName, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    var result = new List<List<DataRow>>();

                    foreach (DataTable table in ds.Tables)
                    {
                        result.Add(table.AsEnumerable().ToList());
                    }

                    return result;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing stored procedure [{procedureName}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling stored procedure [{procedureName}]: {ex.Message}", ex);
            }
        }

        public List<DataRow> ExecuteStoredProcedureRaw(string procedureName, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable.AsEnumerable().ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing stored procedure [{procedureName}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling stored procedure [{procedureName}]: {ex.Message}", ex);
            }
        }

        public int ExecuteNonQuery(string query, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing query [{query}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling query [{query}]: {ex.Message}", ex);
            }
        }

        public int ExecuteNonQueryStoredProcedure(string procedureName, SqlDbTarget dbTarget = SqlDbTarget.Default, SqlParameter[] parameters = null)
        {
            var connection = GetConnection(dbTarget);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlCommand cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error executing stored procedure [{procedureName}]: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unspecified error while calling stored procedure [{procedureName}]: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _connection.Dispose();
            }

            if (_connectionHaiHaCo != null)
            {
                if (_connectionHaiHaCo.State == ConnectionState.Open)
                    _connectionHaiHaCo.Close();
                _connectionHaiHaCo.Dispose();
            }
        }
    }
}
