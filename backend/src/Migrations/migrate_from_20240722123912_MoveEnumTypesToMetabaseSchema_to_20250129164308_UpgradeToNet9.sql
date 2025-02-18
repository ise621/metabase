START TRANSACTION;
ALTER TABLE metabase.component ADD "PrimeDirection_Description" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Exists" boolean;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Exists" boolean;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Abstract" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_ArXiv" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Authors" text[];

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Doi" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Section" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Title" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_Urn" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Publication_WebAddress" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Abstract" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Locator" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Numeration_MainNumber" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Numeration_Prefix" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Numeration_Suffix" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Section" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Standardizers" metabase.standardizer[];

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Title" text;

ALTER TABLE metabase.component ADD "PrimeDirection_Reference_Standard_Year" integer;

ALTER TABLE metabase.component ADD "PrimeSurface_Description" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Exists" boolean;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Exists" boolean;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Abstract" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_ArXiv" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Authors" text[];

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Doi" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Section" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Title" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_Urn" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Publication_WebAddress" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Abstract" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Locator" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Numeration_MainNumber" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Numeration_Prefix" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Numeration_Suffix" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Section" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Standardizers" metabase.standardizer[];

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Title" text;

ALTER TABLE metabase.component ADD "PrimeSurface_Reference_Standard_Year" integer;

ALTER TABLE metabase.component ADD "SwitchableLayers_Description" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Exists" boolean;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Exists" boolean;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Abstract" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_ArXiv" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Authors" text[];

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Doi" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Section" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Title" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_Urn" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Publication_WebAddress" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Abstract" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Locator" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Numeration_MainNumber" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Numeration_Prefix" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Numeration_Suffix" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Section" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Standardizers" metabase.standardizer[];

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Title" text;

ALTER TABLE metabase.component ADD "SwitchableLayers_Reference_Standard_Year" integer;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241202180656_AddPrimeSurfaceAndDirectionAndSurfaceLayersToComponent', '9.0.1');

ALTER TABLE metabase."OpenIddictTokens" DROP CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId";
ALTER TABLE metabase."OpenIddictTokens" DROP CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId";
ALTER TABLE metabase."OpenIddictAuthorizations" DROP CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~";

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "AuthorizationId" TYPE uuid USING "AuthorizationId"::text::uuid;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "ApplicationId" TYPE uuid USING "ApplicationId"::text::uuid;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "Id" TYPE uuid USING "Id"::text::uuid;

ALTER TABLE metabase."OpenIddictScopes" ALTER COLUMN "Id" TYPE uuid USING "Id"::text::uuid;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "ApplicationId" TYPE uuid USING "ApplicationId"::text::uuid;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "Id" TYPE uuid USING "Id"::text::uuid;

ALTER TABLE metabase."OpenIddictApplications" ALTER COLUMN "Id" TYPE uuid USING "Id"::text::uuid;

ALTER TABLE metabase."OpenIddictTokens" ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE RESTRICT;
ALTER TABLE metabase."OpenIddictTokens" ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId" FOREIGN KEY ("AuthorizationId") REFERENCES metabase."OpenIddictAuthorizations" ("Id") ON DELETE RESTRICT;
ALTER TABLE metabase."OpenIddictAuthorizations" ADD CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE RESTRICT;

CREATE TABLE metabase.institution_application (
    "InstitutionId" uuid NOT NULL,
    "ApplicationId" uuid NOT NULL,
    CONSTRAINT "PK_institution_application" PRIMARY KEY ("InstitutionId", "ApplicationId"),
    CONSTRAINT "FK_institution_application_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_institution_application_institution_InstitutionId" FOREIGN KEY ("InstitutionId") REFERENCES metabase.institution ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_institution_application_ApplicationId" ON metabase.institution_application ("ApplicationId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241213072103_AddRelationInstitutionToApplication', '9.0.1');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250129164308_UpgradeToNet9', '9.0.1');

COMMIT;

