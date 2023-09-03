using System.Data;
using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Discount.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly DapperContext _dapperContext;

    public DiscountRepository(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }   
    public async Task<Coupon> GetDiscount(string productName)
    {
        // await using var connection =
            // new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        using var connection = _dapperContext.CreateConnection();
        var coupon = await connection.QueryFirstAsync<Coupon>
        ("SELECT * FROM Coupon WHERE ProductName = @ProductName",
            new { productName = productName });
        if (coupon == null)
        {
            return new Coupon
            {
                ProductName = "No Discount",
                Amount = 0,
                Description = "No Discount Available"
            };
        }
        return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        // await using var connection =
        //     new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        using var connection = _dapperContext.CreateConnection();
        var affected = await connection.ExecuteAsync
        ("INSERT INTO Coupon(ProductName,Description,Amount) VALUES (@ProductName,@Description,@Amount)",
            new Coupon
            {
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount
            });

        if (affected == 0)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        // await using var connection =
        //     new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        using var connection = _dapperContext.CreateConnection();
        var affected = await connection.ExecuteAsync
        ("UPDATE Coupon SET ProductName = @ProductName,Description = @Description,Amount = @Amount WHERE Id = @Id",
            new Coupon
            {
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
                Id = coupon.Id
            });

        if (affected == 0)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        // await using var connection =
        //     new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        using var connection = _dapperContext.CreateConnection();
        var affected = await connection.ExecuteAsync
        ("DELETE FROM Coupon WHERE ProductName = @ProductName",
            new Coupon
            {
                ProductName = productName
            });

        if (affected == 0)
        {
            return false;
        }

        return true;
    }
}