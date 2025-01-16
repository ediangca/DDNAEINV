using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDNAEINV.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchID);
                });

            migrationBuilder.CreateTable(
                name: "BranchTypes",
                columns: table => new
                {
                    BTID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchTypes", x => x.BTID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepID);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    SecID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepID = table.Column<int>(type: "int", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.SecID);
                });


            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.PositionID);
                });

            migrationBuilder.CreateTable(
                name: "UseProfiles",
                columns: table => new
                {
                    ProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Middlename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    DepID = table.Column<int>(type: "int", nullable: false),
                    SecID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UseProfiles", x => x.ProfileID);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UGID = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(1000)", nullable: true, defaultValue: null),
                    isVerified = table.Column<bool>(type: "bit", nullable: true, defaultValue:false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "BranchTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "UseProfiles");

            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}
