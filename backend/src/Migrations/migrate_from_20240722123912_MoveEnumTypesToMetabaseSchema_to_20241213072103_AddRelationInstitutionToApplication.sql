START TRANSACTION;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "AuthorizationId" TYPE uuid;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "ApplicationId" TYPE uuid;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "Id" TYPE uuid;

ALTER TABLE metabase."OpenIddictScopes" ALTER COLUMN "Id" TYPE uuid;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "ApplicationId" TYPE uuid;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "Id" TYPE uuid;

ALTER TABLE metabase."OpenIddictApplications" ALTER COLUMN "Id" TYPE uuid;

CREATE TABLE metabase.institution_application (
    "InstitutionId" uuid NOT NULL,
    "ApplicationId" uuid NOT NULL,
    CONSTRAINT "PK_institution_application" PRIMARY KEY ("InstitutionId", "ApplicationId"),
    CONSTRAINT "FK_institution_application_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_institution_application_institution_InstitutionId" FOREIGN KEY ("InstitutionId") REFERENCES metabase.institution ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_institution_application_ApplicationId" ON metabase.institution_application ("ApplicationId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241213072103_AddRelationInstitutionToApplication', '8.0.10');

COMMIT;

