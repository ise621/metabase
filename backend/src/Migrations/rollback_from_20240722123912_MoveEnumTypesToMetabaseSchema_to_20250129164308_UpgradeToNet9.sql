START TRANSACTION;
DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250129164308_UpgradeToNet9';

DROP TABLE metabase.institution_application;

ALTER TABLE metabase."OpenIddictTokens" DROP CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId";
ALTER TABLE metabase."OpenIddictTokens" DROP CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId";
ALTER TABLE metabase."OpenIddictAuthorizations" DROP CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~";

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "AuthorizationId" TYPE text;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "ApplicationId" TYPE text;

ALTER TABLE metabase."OpenIddictTokens" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictScopes" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "ApplicationId" TYPE text;

ALTER TABLE metabase."OpenIddictAuthorizations" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictApplications" ALTER COLUMN "Id" TYPE text;

ALTER TABLE metabase."OpenIddictTokens" ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE RESTRICT;
ALTER TABLE metabase."OpenIddictTokens" ADD CONSTRAINT "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId" FOREIGN KEY ("AuthorizationId") REFERENCES metabase."OpenIddictAuthorizations" ("Id") ON DELETE RESTRICT;
ALTER TABLE metabase."OpenIddictAuthorizations" ADD CONSTRAINT "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~" FOREIGN KEY ("ApplicationId") REFERENCES metabase."OpenIddictApplications" ("Id") ON DELETE RESTRICT;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20241213072103_AddRelationInstitutionToApplication';

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Description";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Exists";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Exists";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Abstract";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_ArXiv";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Authors";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Doi";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Section";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Title";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_Urn";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Publication_WebAddress";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Abstract";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Locator";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Numeration_MainNumber";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Numeration_Prefix";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Numeration_Suffix";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Section";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Standardizers";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Title";

ALTER TABLE metabase.component DROP COLUMN "PrimeDirection_Reference_Standard_Year";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Description";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Exists";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Exists";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Abstract";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_ArXiv";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Authors";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Doi";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Section";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Title";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_Urn";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Publication_WebAddress";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Abstract";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Locator";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Numeration_MainNumber";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Numeration_Prefix";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Numeration_Suffix";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Section";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Standardizers";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Title";

ALTER TABLE metabase.component DROP COLUMN "PrimeSurface_Reference_Standard_Year";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Description";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Exists";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Exists";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Abstract";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_ArXiv";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Authors";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Doi";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Section";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Title";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_Urn";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Publication_WebAddress";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Abstract";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Locator";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Numeration_MainNumber";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Numeration_Prefix";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Numeration_Suffix";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Section";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Standardizers";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Title";

ALTER TABLE metabase.component DROP COLUMN "SwitchableLayers_Reference_Standard_Year";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20241202180656_AddPrimeSurfaceAndDirectionAndSurfaceLayersToComponent';

COMMIT;

