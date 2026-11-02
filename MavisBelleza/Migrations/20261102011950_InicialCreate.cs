using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MavisBelleza.Migrations
{
    /// <inheritdoc />
    public partial class InicialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    DuracionMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    Precio = table.Column<double>(type: "REAL", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Horario = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    Precio = table.Column<double>(type: "REAL", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turnos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turnos_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Apellido", "Email", "FechaAlta", "Nombre", "Notas", "Telefono" },
                values: new object[,]
                {
                    { 1, "Gómez", "ana.gomez@example.com", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Ana", null, "11 2345 6789" },
                    { 2, "Fernández", "lucia.fernandez@example.com", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Lucía", null, "11 3456 7890" },
                    { 3, "Martínez", "sofia.martinez@example.com", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Sofía", null, "11 4567 8901" },
                    { 4, "López", "valentina.lopez@example.com", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Valentina", null, "11 5678 9012" },
                    { 5, "Rodríguez", "camila.rodriguez@example.com", new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Camila", null, "11 6789 0123" }
                });

            migrationBuilder.InsertData(
                table: "Servicios",
                columns: new[] { "Id", "Activo", "Descripcion", "DuracionMinutos", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, true, "Arquea y da curvatura natural a las pestañas desde la raíz. Incluye nutrición con botox/keratina y tinte.", 90, "Lifting de Pestañas", 30000.0 },
                    { 2, true, "Diseño personalizado según la morfología del rostro (visagismo), con pinza o cera suave.", 60, "Laminado de Cejas", 30000.0 },
                    { 4, true, "Lifting de pestañas con nutrición y tinte, más perfilado de cejas personalizado.", 90, "Promo Lifting + Perfilado", 40000.0 },
                    { 5, true, "Laminado de cejas con hidratación, más lifting de pestañas con nutrición y tinte.", 90, "Promo Laminado + Lifting", 55000.0 }
                });

            migrationBuilder.InsertData(
                table: "Turnos",
                columns: new[] { "Id", "ClienteId", "Estado", "Fecha", "FechaCreacion", "Horario", "MetodoPago", "Notas", "Precio", "ServicioId" },
                values: new object[,]
                {
                    { 1, 1, "Realizado", new DateTime(2026, 10, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "10:00", "Efectivo", null, 30000.0, 1 },
                    { 2, 2, "Realizado", new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "11:30", "Transferencia", null, 15000.0, 3 },
                    { 3, 3, "Confirmado", new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "15:00", "Mercado Pago", null, 30000.0, 2 },
                    { 4, 1, "Pendiente", new DateTime(2026, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "09:30", "Efectivo", null, 15000.0, 3 },
                    { 5, 4, "Confirmado", new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "16:30", "Transferencia", null, 30000.0, 1 },
                    { 6, 5, "Cancelado", new DateTime(2026, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "12:00", "Efectivo", null, 30000.0, 2 },
                    { 7, 2, "Confirmado", new DateTime(2026, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "10:30", "Mercado Pago", null, 30000.0, 1 },
                    { 8, 3, "Pendiente", new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), "17:00", "Transferencia", null, 15000.0, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Email",
                table: "Clientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ClienteId",
                table: "Turnos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_Fecha_Horario",
                table: "Turnos",
                columns: new[] { "Fecha", "Horario" });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ServicioId",
                table: "Turnos",
                column: "ServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turnos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Servicios");
        }
    }
}
