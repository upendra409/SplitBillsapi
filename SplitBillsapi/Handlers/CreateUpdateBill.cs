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
    public class CreateUpdateBill : IRequestHandler<CreateUpdateBillRequest, CreateUpdateBillResponse>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public CreateUpdateBill(IConfiguration configuration)
        {
            _connectionString = Configuration.GetConnectionString(configuration);
            helper = new Helper(configuration);
        }
        public async Task<CreateUpdateBillResponse> Handle(CreateUpdateBillRequest request, CancellationToken cancellationToken)
        {
            try
            {
                DataTable dataTable = helper.GetParticipantTable();
                CreateUpdateBillResponse createBillResponse = new CreateUpdateBillResponse();
                foreach (Participant participant in request.Participants)
                {
                    dataTable.Rows.Add(participant.SignInName, participant.Rate, participant.TransactionType);
                }
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    if (request.TransactionId == -1)
                    {
                        using (var command = new SqlCommand("sp_InsertTransactions", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandTimeout = 60;
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@TransactionName",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.TransactionName
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupId",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Int,
                                Value = request.GroupId
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Amount",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.SmallMoney,
                                Value = request.Amount
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Participants",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Structured,
                                Value = dataTable
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Currency",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.Currency
                            });
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                createBillResponse.TransactionId = await reader.IsDBNullAsync(0) ? -1 : await reader.GetFieldValueAsync<int>(0);
                            }
                        }
                    }
                    else
                    {
                        using (var command = new SqlCommand("sp_UpdateTransactions", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandTimeout = 60;
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@TransactionOwner",
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
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@TransactionName",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.TransactionName
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupId",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Int,
                                Value = request.GroupId
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Amount",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.SmallMoney,
                                Value = request.Amount
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Participants",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Structured,
                                Value = dataTable
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@Currency",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.Currency
                            });
                            var returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;
                            var reader = command.ExecuteReader();
                            if (Int32.Parse(returnParameter.Value.ToString()) == 0)
                            {
                                createBillResponse.Message = "Not authorized or transaction not found";
                                createBillResponse.TransactionId = request.TransactionId;
                            }
                            else
                            {
                                createBillResponse.Message = "Operation successfull";
                                createBillResponse.TransactionId = request.TransactionId;
                            }
                            //while (reader.Read())
                            //{
                            //    createBillResponse.TransactionId = await reader.IsDBNullAsync(0) ? -1 : await reader.GetFieldValueAsync<int>(0);
                            //}
                        }
                    }
                }
                return createBillResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
