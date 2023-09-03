using Discount.Core.Entities;
using FluentMigrator;

namespace Discount.API.Migrations;

[Migration(202309010002)]
public class InitialSeed_202309010002 : Migration
{
    public override void Down()
    {
        Delete.FromTable("Coupon")
            .Row(new Coupon 
            {
                ProductName = "Adidas Quick Force Indoor Badminton Shoes",
            });

        Delete.FromTable("Coupon")
            .Row(new Coupon 
            {
                ProductName = "Yonex VCORE Pro 400 A Tennis Racquet (270gm,Strung)",
            });
    }

    public override void Up()
    {
        Execute.Sql("INSERT INTO \"Coupon\" (\"ProductName\", \"Description\", \"Amount\") VALUES ('Sample Product 1', 'Description 1', 100)");

        Execute.Sql(
            "INSERT INTO \"Coupon\" (\"ProductName\", \"Description\", \"Amount\") VALUES ('Sample Product 2', 'Description 2', 200)");

        // Insert.IntoTable("Coupon")
        //     .Row(new Coupon 
        //     {
        //         ProductName = "Adidas Quick Force Indoor Badminton Shoes",
        //         Description = "Shoe Discount",
        //         Amount = 500
        //     });
        //
        // Insert.IntoTable("Coupon")
        //     .Row(new Coupon 
        //     {
        //         ProductName = "Yonex VCORE Pro 400 A Tennis Racquet (270gm,Strung)",
        //         Description = "Racquet Discount",
        //         Amount = 700
        //     });
    }
}