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
    public class GetBill : IRequestHandler<GetBillRequest, GetBillResponse>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public GetBill(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TestDatabase");
            helper = new Helper(configuration);
        }
        public async Task<GetBillResponse> Handle(GetBillRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetBillResponse getBillResponse = new GetBillResponse();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_GetTransactionDetails", connection))
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
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60;
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@TransactionId",
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.Int,
                            Value = request.TransactionId
                        });
                        var reader = command.ExecuteReader();
                        if(!reader.HasRows)
                        { return null; }
                        while (reader.Read())
                        {
                            getBillResponse.SignInName = await reader.IsDBNullAsync(0) ? string.Empty : await reader.GetFieldValueAsync<string>(0);
                            getBillResponse.GroupName = await reader.IsDBNullAsync(1) ? string.Empty : await reader.GetFieldValueAsync<string>(1);
                            getBillResponse.TransactionName = await reader.IsDBNullAsync(2) ? string.Empty : await reader.GetFieldValueAsync<string>(2);
                            getBillResponse.Amount = await reader.IsDBNullAsync(3) ? 0 : await reader.GetFieldValueAsync<decimal>(3);
                            getBillResponse.Currency = await reader.IsDBNullAsync(4) ? string.Empty : await reader.GetFieldValueAsync<string>(4);
                            getBillResponse.CreatedOn = await reader.IsDBNullAsync(5) ? System.DateTime.Now : await reader.GetFieldValueAsync<DateTime>(5);
                        }
                            
                            }
                }
                return getBillResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
