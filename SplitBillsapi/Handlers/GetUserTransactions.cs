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
    public class GetUserTransactions : IRequestHandler<GetUserTransactionRequest, GetUserTransactionsResponse>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public GetUserTransactions(IConfiguration configuration)
        {
            _connectionString = Configuration.GetConnectionString(configuration);
            helper = new Helper(configuration);
        }

        public async Task<GetUserTransactionsResponse> Handle(GetUserTransactionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetUserTransactionsResponse getUserTransactionsResponse = new GetUserTransactionsResponse();
                List<UserTransactionDetails> listUserTransactionDetails = new List<UserTransactionDetails>();
                string nativeCurrency = string.Empty;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_GetTransactionsForUser", connection))
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
                        var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        { return null; }
                        while (reader.Read())
                        {
                            UserTransactionDetails userTransactionDetails = new UserTransactionDetails();
                            userTransactionDetails.Transactionid = await reader.IsDBNullAsync(0) ? 0 : await reader.GetFieldValueAsync<int>(0);
                            userTransactionDetails.TransactionName = await reader.IsDBNullAsync(1) ? string.Empty : await reader.GetFieldValueAsync<string>(1);
                            userTransactionDetails.Amount = await reader.IsDBNullAsync(2) ? 0 : await reader.GetFieldValueAsync<decimal>(2);
                            nativeCurrency = await reader.IsDBNullAsync(3) ? string.Empty : await reader.GetFieldValueAsync<string>(3);
                            userTransactionDetails.TransactionDate = await reader.IsDBNullAsync(4) ? System.DateTime.Now : await reader.GetFieldValueAsync<System.DateTime>(4);

                            listUserTransactionDetails.Add(userTransactionDetails);
                        }

                        //var query = listUserTransactionDetails.AsQueryable();
                        //var filterBy = request.filteringParams.FilterBy.Trim().ToLowerInvariant();
                        //List<decimal> d = new List<decimal>();
                        ////d.Where<>
                        //if (!string.IsNullOrEmpty(filterBy))
                        //{
                        //    //query = listUserTransactionDetails
                        //    //       .Where(m => m.LeadActor.ToLowerInvariant().Contains(filterBy)
                        //    //       || m.Title.ToLowerInvariant().Contains(filterBy)
                        //    //       || m.Summary.ToLowerInvariant().Contains(filterBy));
                        //    //query = query
                        //    //     .Where(m => m.TransactionName.ToLowerInvariant().Contains(filterBy));
                        //    //query = query.Where(m => m.Amount.CompareTo(10.0);
                        //}

                        getUserTransactionsResponse.Currency = nativeCurrency;
                        //getUserTransactionsResponse.UserTransactionDetails = new UserTransactionDetails();
                        getUserTransactionsResponse.UserTransactionDetails = listUserTransactionDetails;
                    }
                }
                return getUserTransactionsResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
