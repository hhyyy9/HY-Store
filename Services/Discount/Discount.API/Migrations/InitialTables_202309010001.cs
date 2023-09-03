using FluentMigrator;
using FluentMigrator.Postgres;

namespace Discount.API.Migrations;

[Migration(202309010001)]
public class InitialTables_202106280001 : Migration
{
    public override void Down()
    {
        Delete.Table("Coupon");
    }

    public override void Up()
    {
        Create.Table("Coupon")
            .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("ProductName").AsString(500).NotNullable()
            .WithColumn("Description").AsString(8000)
            .WithColumn("Amount").AsInt64().NotNullable();
    }
}