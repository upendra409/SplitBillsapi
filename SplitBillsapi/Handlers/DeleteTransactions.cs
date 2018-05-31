using MediatR;
using Microsoft.Extensions.Configuration;
using SplitBillsapi.Contracts;
using SplitBillsapi.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SplitBillsapi.Handlers
{
    public class DeleteTransactions : IRequestHandler<DeleteTransactionRequest, DeleteTransactionRequest>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public DeleteTransactions(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TestDatabase");
            helper = new Helper(configuration);
        }
        public async Task<DeleteTransactionRequest> Handle(DeleteTransactionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string response = string.Empty;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_DeleteTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60;
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@SignInName",
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            Value = request.SignInName
                        });
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@TransactionId",
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.Int,
                            Value = request.TransactionId
                        });
                        var returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        var reader = command.ExecuteReader();
                        if (Int32.Parse(returnParameter.Value.ToString()) == 0)
                        {
                            request.Response = "Successfully deleted the transaction";
                        }
                        else
                        {
                            return null;
                        }
                        //while (reader.Read())
                        //{
                        //}
                    }
                }
                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
