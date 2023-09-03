using Dapper;
using Discount.Infrastructure.Context;
using Npgsql;

namespace Discount.Infrastructure.Migrations;

public class Database
{
    private readonly DapperContext _context;

    public Database(DapperContext context)
    {
        _context = context;
    }
}