using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Logging
{
    public partial class Net6Upgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "UserActivityHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlayerId",
                table: "UserActivityHistory",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "UserActivityHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "TradeHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "TradeId",
                table: "TradeHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "TargetId",
                table: "TradeHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlayerId",
                table: "TradeHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "TradeHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "RequestLogs",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "GuildHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlayerId",
                table: "GuildHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "InitiatorId",
                table: "GuildHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "GuildId",
                table: "GuildHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "GuildHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserId",
                table: "ChatHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "TargetId",
                table: "ChatHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PlayerId",
                table: "ChatHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Id",
                table: "ChatHistory",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserActivityHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "UserActivityHistory",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UserActivityHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "TradeId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "RequestLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "GuildHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "GuildHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "InitiatorId",
                table: "GuildHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "GuildId",
                table: "GuildHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "GuildHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ChatHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetId",
                table: "ChatHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "PlayerId",
                table: "ChatHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ChatHistory",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");
        }
    }
}
