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
    public class GetGroups : IRequestHandler<GetGroupRequest, GetGroupResponse>
    {
        private string _connectionString = string.Empty;
        private readonly IConfiguration _configuration;
        private Helper helper;
        public GetGroups(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("TestDatabase");
            helper = new Helper(configuration);
        }
        public async Task<GetGroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                GetGroupResponse getGroupResponse = new GetGroupResponse();
                List<Member> members = new List<Member>();
                var groupName = string.Empty;
                var createdOn = System.DateTime.Now;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("sp_GetGroupDetails", connection))
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
                        var reader = command.ExecuteReader();
                        if (!reader.HasRows)
                        { return null; }
                        while (reader.Read())
                        {
                            Member member = new Member();
                            groupName = await reader.IsDBNullAsync(0) ? string.Empty : await reader.GetFieldValueAsync<string>(0);
                            member.SignInName = await reader.IsDBNullAsync(1) ? string.Empty : await reader.GetFieldValueAsync<string>(1);
                            member.Name = await reader.IsDBNullAsync(2) ? string.Empty : await reader.GetFieldValueAsync<string>(2);
                            member.Currency = await reader.IsDBNullAsync(3) ? string.Empty : await reader.GetFieldValueAsync<string>(3);
                            createdOn = await reader.IsDBNullAsync(4) ? System.DateTime.Now : await reader.GetFieldValueAsync<DateTime>(4);
                            members.Add(member);
                        }
                    }
                    getGroupResponse.ListMembers = new List<Member>();
                    getGroupResponse.ListMembers = members;
                    getGroupResponse.CreatedOn = createdOn;
                    getGroupResponse.GroupName = groupName;
                }
                return getGroupResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
