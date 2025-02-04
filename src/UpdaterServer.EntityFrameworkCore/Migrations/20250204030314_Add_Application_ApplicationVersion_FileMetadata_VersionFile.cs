using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UpdaterServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Application_ApplicationVersion_FileMetadata_VersionFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "UpdaterServer");

            migrationBuilder.CreateTable(
                name: "AppApplications",
                schema: "UpdaterServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppFileMetadatas",
                schema: "UpdaterServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Path = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFileMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppApplicationVersions",
                schema: "UpdaterServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApplicationVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppApplicationVersions_AppApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "UpdaterServer",
                        principalTable: "AppApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppVersionFiles",
                schema: "UpdaterServer",
                columns: table => new
                {
                    VersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileMetadataId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersionFiles", x => new { x.VersionId, x.FileMetadataId });
                    table.ForeignKey(
                        name: "FK_AppVersionFiles_AppApplicationVersions_ApplicationVersionId",
                        column: x => x.ApplicationVersionId,
                        principalSchema: "UpdaterServer",
                        principalTable: "AppApplicationVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppVersionFiles_AppApplicationVersions_VersionId",
                        column: x => x.VersionId,
                        principalSchema: "UpdaterServer",
                        principalTable: "AppApplicationVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppVersionFiles_AppFileMetadatas_FileMetadataId",
                        column: x => x.FileMetadataId,
                        principalSchema: "UpdaterServer",
                        principalTable: "AppFileMetadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppApplicationVersions_ApplicationId",
                schema: "UpdaterServer",
                table: "AppApplicationVersions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVersionFiles_ApplicationVersionId",
                schema: "UpdaterServer",
                table: "AppVersionFiles",
                column: "ApplicationVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVersionFiles_FileMetadataId",
                schema: "UpdaterServer",
                table: "AppVersionFiles",
                column: "FileMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVersionFiles_VersionId_FileMetadataId",
                schema: "UpdaterServer",
                table: "AppVersionFiles",
                columns: new[] { "VersionId", "FileMetadataId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersionFiles",
                schema: "UpdaterServer");

            migrationBuilder.DropTable(
                name: "AppApplicationVersions",
                schema: "UpdaterServer");

            migrationBuilder.DropTable(
                name: "AppFileMetadatas",
                schema: "UpdaterServer");

            migrationBuilder.DropTable(
                name: "AppApplications",
                schema: "UpdaterServer");
        }
    }
}
