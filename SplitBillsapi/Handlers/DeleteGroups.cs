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
    public class DeleteGroups : IRequestHandler<DeleteGroupRequest, string>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public DeleteGroups(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TestDatabase");
            helper = new Helper(configuration);
        }
        public async Task<string> Handle(DeleteGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string response = string.Empty;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_DeleteGroup", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60;
                        command.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@GroupId",
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.Int,
                            Value = request.GroupId
                        });
                        var returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        var reader = command.ExecuteReader();
                        if (Int32.Parse(returnParameter.Value.ToString()) == 0)
                        {
                            response = "Successfully deleted the group";
                        }
                        else
                        {
                            response = "Not authorized or group not found";
                        }
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
