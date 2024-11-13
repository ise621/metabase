using System;
using Metabase.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Metabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        private static readonly string[] _openIddictAuthorizationsIndexByApplicationIdStatusSubjectTypeColumns = ["ApplicationId", "Status", "Subject", "Type"];
        private static readonly string[] _oppenIddictTokensIndexByApplicationIdStatusSubjectTypeColumns = ["ApplicationId", "Status", "Subject", "Type"];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "metabase");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .Annotation("Npgsql:Enum:public.institution_representative_role", "owner,maintainer,assistant")
                .Annotation("Npgsql:Enum:public.institution_state", "unknown,operative,inoperative")
                .Annotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .Annotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "component",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Availability = table.Column<NpgsqlRange<DateTime>>(type: "tsrange", nullable: true),
                    Categories = table.Column<ComponentCategory[]>(type: "component_category[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "institution",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    WebsiteLocator = table.Column<string>(type: "text", nullable: true),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<InstitutionState>(type: "institution_state", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "method",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Standard_Title = table.Column<string>(type: "text", nullable: true),
                    Standard_Abstract = table.Column<string>(type: "text", nullable: true),
                    Standard_Section = table.Column<string>(type: "text", nullable: true),
                    Standard_Year = table.Column<int>(type: "integer", nullable: true),
                    Standard_Numeration_Prefix = table.Column<string>(type: "text", nullable: true),
                    Standard_Numeration_MainNumber = table.Column<string>(type: "text", nullable: true),
                    Standard_Numeration_Suffix = table.Column<string>(type: "text", nullable: true),
                    Standard_Standardizers = table.Column<Standardizer[]>(type: "standardizer[]", nullable: true),
                    Standard_Locator = table.Column<string>(type: "text", nullable: true),
                    Publication_Title = table.Column<string>(type: "text", nullable: true),
                    Publication_Abstract = table.Column<string>(type: "text", nullable: true),
                    Publication_Section = table.Column<string>(type: "text", nullable: true),
                    Publication_Authors = table.Column<string[]>(type: "text[]", nullable: true),
                    Publication_Doi = table.Column<string>(type: "text", nullable: true),
                    Publication_ArXiv = table.Column<string>(type: "text", nullable: true),
                    Publication_Urn = table.Column<string>(type: "text", nullable: true),
                    Publication_WebAddress = table.Column<string>(type: "text", nullable: true),
                    Validity = table.Column<NpgsqlRange<DateTime>>(type: "tsrange", nullable: true),
                    Availability = table.Column<NpgsqlRange<DateTime>>(type: "tsrange", nullable: true),
                    CalculationLocator = table.Column<string>(type: "text", nullable: true),
                    Categories = table.Column<MethodCategory[]>(type: "method_category[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_method", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictApplications",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedirectUris = table.Column<string>(type: "text", nullable: true),
                    Requirements = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Descriptions = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    DisplayNames = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PostalAddress = table.Column<string>(type: "text", nullable: true),
                    WebsiteLocator = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "component_concretization_and_generalization",
                schema: "metabase",
                columns: table => new
                {
                    GeneralComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcreteComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component_concretization_and_generalization", x => new { x.GeneralComponentId, x.ConcreteComponentId });
                    table.ForeignKey(
                        name: "FK_component_concretization_and_generalization_component_Concr~",
                        column: x => x.ConcreteComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_component_concretization_and_generalization_component_Gener~",
                        column: x => x.GeneralComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "component_manufacturer",
                schema: "metabase",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_component_manufacturer", x => new { x.ComponentId, x.InstitutionId });
                    table.ForeignKey(
                        name: "FK_component_manufacturer_component_ComponentId",
                        column: x => x.ComponentId,
                        principalSchema: "metabase",
                        principalTable: "component",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_component_manufacturer_institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalSchema: "metabase",
                        principalTable: "institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "database",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Locator = table.Column<string>(type: "text", nullable: false),
                    OperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database", x => x.Id);
                    table.ForeignKey(
                        name: "FK_database_institution_OperatorId",
                        column: x => x.OperatorId,
                        principalSchema: "metabase",
                        principalTable: "institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataFormats",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<string>(type: "text", nullable: false),
                    SchemaLocator = table.Column<string>(type: "text", nullable: true),
                    Standard_Title = table.Column<string>(type: "text", nullable: true),
                    Standard_Abstract = table.Column<string>(type: "text", nullable: true),
                    Standard_Section = table.Column<string>(type: "text", nullable: true),
                    Standard_Year = table.Column<int>(type: "integer", nullable: true),
                    Standard_Numeration_Prefix = table.Column<string>(type: "text", nullable: true),
                    Standard_Numeration_MainNumber = table.Column<string>(type: "text", nullable: true),
                    Standard_Numeration_Suffix = table.Column<string>(type: "text", nullable: true),
                    Standard_Standardizers = table.Column<Standardizer[]>(type: "standardizer[]", nullable: true),
                    Standard_Locator = table.Column<string>(type: "text", nullable: true),
                    Publication_Title = table.Column<string>(type: "text", nullable: true),
                    Publication_Abstract = table.Column<string>(type: "text", nullable: true),
                    Publication_Section = table.Column<string>(type: "text", nullable: true),
                    Publication_Authors = table.Column<string[]>(type: "text[]", nullable: true),
                    Publication_Doi = table.Column<string>(type: "text", nullable: true),
                    Publication_ArXiv = table.Column<string>(type: "text", nullable: true),
                    Publication_Urn = table.Column<string>(type: "text", nullable: true),
                    Publication_WebAddress = table.Column<string>(type: "text", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFormats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataFormats_institution_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "metabase",
                        principalTable: "institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "institution_method_developer",
                schema: "metabase",
                columns: table => new
                {
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_method_developer", x => new { x.InstitutionId, x.MethodId });
                    table.ForeignKey(
                        name: "FK_institution_method_developer_institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalSchema: "metabase",
                        principalTable: "institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_institution_method_developer_method_MethodId",
                        column: x => x.MethodId,
                        principalSchema: "metabase",
                        principalTable: "method",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictAuthorizations",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                        column: x => x.ApplicationId,
                        principalSchema: "metabase",
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claim_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "metabase",
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "institution_representative",
                schema: "metabase",
                columns: table => new
                {
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<InstitutionRepresentativeRole>(type: "institution_representative_role", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_representative", x => new { x.InstitutionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_institution_representative_institution_InstitutionId",
                        column: x => x.InstitutionId,
                        principalSchema: "metabase",
                        principalTable: "institution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_institution_representative_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claim_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "metabase",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_login_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_method_developer",
                schema: "metabase",
                columns: table => new
                {
                    MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_method_developer", x => new { x.UserId, x.MethodId });
                    table.ForeignKey(
                        name: "FK_user_method_developer_method_MethodId",
                        column: x => x.MethodId,
                        principalSchema: "metabase",
                        principalTable: "method",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_method_developer_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "metabase",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_role_role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "metabase",
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "metabase",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_token", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "metabase",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                schema: "metabase",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApplicationId = table.Column<string>(type: "text", nullable: true),
                    AuthorizationId = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    RedemptionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "metabase",
                        principalTable: "OpenIddictApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                        column: x => x.AuthorizationId,
                        principalSchema: "metabase",
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_component_concretization_and_generalization_ConcreteCompone~",
                schema: "metabase",
                table: "component_concretization_and_generalization",
                column: "ConcreteComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_component_manufacturer_InstitutionId",
                schema: "metabase",
                table: "component_manufacturer",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_database_OperatorId",
                schema: "metabase",
                table: "database",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFormats_ManagerId",
                schema: "metabase",
                table: "DataFormats",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_institution_method_developer_MethodId",
                schema: "metabase",
                table: "institution_method_developer",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_institution_representative_UserId",
                schema: "metabase",
                table: "institution_representative",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictApplications_ClientId",
                schema: "metabase",
                table: "OpenIddictApplications",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
                schema: "metabase",
                table: "OpenIddictAuthorizations",
                columns: _openIddictAuthorizationsIndexByApplicationIdStatusSubjectTypeColumns);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictScopes_Name",
                schema: "metabase",
                table: "OpenIddictScopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
                schema: "metabase",
                table: "OpenIddictTokens",
                columns: _oppenIddictTokensIndexByApplicationIdStatusSubjectTypeColumns);

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_AuthorizationId",
                schema: "metabase",
                table: "OpenIddictTokens",
                column: "AuthorizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddictTokens_ReferenceId",
                schema: "metabase",
                table: "OpenIddictTokens",
                column: "ReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "metabase",
                table: "role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claim_RoleId",
                schema: "metabase",
                table: "role_claim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "metabase",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "metabase",
                table: "user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claim_UserId",
                schema: "metabase",
                table: "user_claim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_login_UserId",
                schema: "metabase",
                table: "user_login",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_method_developer_MethodId",
                schema: "metabase",
                table: "user_method_developer",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_RoleId",
                schema: "metabase",
                table: "user_role",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "component_concretization_and_generalization",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "component_manufacturer",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "database",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "DataFormats",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "institution_method_developer",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "institution_representative",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "OpenIddictScopes",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user_login",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user_method_developer",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user_token",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "component",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "institution",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "method",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "role",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "user",
                schema: "metabase");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications",
                schema: "metabase");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:public.component_category", "material,layer,unit")
                .OldAnnotation("Npgsql:Enum:public.institution_representative_role", "owner,maintainer,assistant")
                .OldAnnotation("Npgsql:Enum:public.institution_state", "unknown,operative,inoperative")
                .OldAnnotation("Npgsql:Enum:public.method_category", "measurement,calculation")
                .OldAnnotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
                .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");
        }
    }
}
