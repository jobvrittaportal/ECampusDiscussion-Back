using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QA_Backend.Migrations
{
    /// <inheritdoc />
    public partial class initial01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsFeature = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role_Mast",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Mast", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login_ID = table.Column<int>(type: "int", nullable: false),
                    Login_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Login_Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Status = table.Column<bool>(type: "bit", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Details", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Page_PageId",
                        column: x => x.PageId,
                        principalTable: "Page",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permission_Role_Mast_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role_Mast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permission_Table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Roles_ID = table.Column<int>(type: "int", nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_Table", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_Table_Role_Mast_Roles_ID",
                        column: x => x.Roles_ID,
                        principalTable: "Role_Mast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login_ID = table.Column<int>(type: "int", nullable: false),
                    Role_ID = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created_By_Id = table.Column<int>(type: "int", nullable: true),
                    Updated_By_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Roles_Role_Mast_Role_ID",
                        column: x => x.Role_ID,
                        principalTable: "Role_Mast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PageId",
                table: "Permission",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_RoleId",
                table: "Permission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Table_Roles_ID",
                table: "Permission_Table",
                column: "Roles_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_Role_ID",
                table: "User_Roles",
                column: "Role_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Permission_Table");

            migrationBuilder.DropTable(
                name: "User_Details");

            migrationBuilder.DropTable(
                name: "User_Roles");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Role_Mast");
        }
    }
}
