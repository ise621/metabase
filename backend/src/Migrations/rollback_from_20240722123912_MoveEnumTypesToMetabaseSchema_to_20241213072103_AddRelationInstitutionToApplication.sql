START TRANSACTION;

DROP TABLE metabase.institution_application;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "AuthorizationId" TYPE text;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "ApplicationId" TYPE text;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictScopes" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "ApplicationId" TYPE text;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictApplications" ALTER COLUMN "Id" TYPE text;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20241213072103_AddRelationInstitutionToApplication';

COMMIT;

