using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dash.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", nullable: false),
                    email_verified = table.Column<bool>(type: "INTEGER", nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_login_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    password_changed_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    timezone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    preferred_language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    failed_login_attempts = table.Column<int>(type: "INTEGER", nullable: false),
                    last_login_ip = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    password_reset_token_hash = table.Column<string>(type: "TEXT", nullable: true),
                    password_reset_expires_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "login_attempts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    attempted_at = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ip_address = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    user_agent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    is_successful = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_login_attempts", x => x.id);
                    table.ForeignKey(
                        name: "f_k_login_attempts__users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_login_attempts_attempted_at",
                table: "login_attempts",
                column: "attempted_at");

            migrationBuilder.CreateIndex(
                name: "i_x_login_attempts_user_id",
                table: "login_attempts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "login_attempts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
