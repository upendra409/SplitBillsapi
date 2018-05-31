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
    public class CreateorUpdateGroups : IRequestHandler<CreateUpdateGroupRequest, int>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public CreateorUpdateGroups(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TestDatabase");
            helper = new Helper(configuration);
        }
        public async Task<int> Handle(CreateUpdateGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                DataTable dataTable = helper.GetMemberTable();
                CreateUpdateBillResponse createBillResponse = new CreateUpdateBillResponse();
                int groupId = 0;
                foreach (Member member in request.listMembers)
                {
                    dataTable.Rows.Add(member.SignInName, member.Name, member.Currency);
                }
                if (request.GroupId == -1)
                {
                    #region create
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("sp_InsertGroupsandMembers", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandTimeout = 60;
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupName",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.GroupName
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupMembers",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Structured,
                                Value = dataTable
                            });
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                groupId = await reader.IsDBNullAsync(0) ? -1 : await reader.GetFieldValueAsync<int>(0);
                            }
                        }
                    }
                    return groupId;
                    #endregion
                }
                else
                {
                    #region update
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("sp_UpdateGroupsandMembers", connection))
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
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupName",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                Value = request.GroupName
                            });
                            command.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@GroupMembers",
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Structured,
                                Value = dataTable
                            });
                            var returnParameter = command.Parameters.Add("@ReturnValue", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;
                            var reader = command.ExecuteReader();
                            return Int32.Parse(returnParameter.Value.ToString());
                            //while (reader.Read())
                            //{
                            //    groupId = await reader.IsDBNullAsync(0) ? -1 : await reader.GetFieldValueAsync<int>(0);
                            //}
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
