using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Infrastructure.Context;

public class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlConnection CreateConnection()
        => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
}