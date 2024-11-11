# Concise introduction to GNU Make:
# https://swcarpentry.github.io/make-novice/reference.html

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':|##' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

# ------------------------------------------------ #
# Tasks to run, for example, in a Docker container #
# ------------------------------------------------ #

# List of dotnet commands
# https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet

# See https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test
# dotnet test --filter "FullyQualifiedName~${x}" --verbosity minimal
test : ## Run tests
	@echo "--------------------------------------------------"
	@echo "Complete test list"
	@echo "--------------------------------------------------"
	dotnet test \
		--list-tests
	@echo "--------------------------------------------------"
	@echo "All test runs"
	@echo "--------------------------------------------------"
	dotnet test \
		--collect:"XPlat Code Coverage"
.PHONY : test

assets : ## Build assets
	npm run all
.PHONY : assets

# For usage information see https://github.com/tomchavakis/nuget-license#usage
# or run `dotnet tool run dotnet-project-licenses -- --help`
licenses : ## Print licenses
	dotnet tool run \
		dotnet-project-licenses \
			-i ./Metabase.sln
			# --json
			# --allowed-license-types ...
			# --forbidden-license-types ...
.PHONY : licenses

# https://github.com/dotnet/diagnostics/blob/master/documentation/dotnet-trace-instructions.md#using-dotnet-trace
trace : ## Trace the dotnet process `Metabase`
	dotnet tool run \
		dotnet-trace collect \
			--process-id $$(dotnet tool run dotnet-trace ps | grep Metabase | awk '{print $$1}') \
			--format speedscope \
			--providers Microsoft-Windows-DotNETRuntime
.PHONY : trace

# If `graphqurl` were installed, the schema could be introspected with
# `gq http://backend:8080/graphql/ --introspect`
# See https://github.com/hasura/graphqurl#export-schema
update-schema : ## Update GraphQl schema
	wget \
		--output-document ./test/Integration/GraphQl/__snapshots__/GraphQlSchemaTests.IsUnchanged.snap \
		http://backend:8080/graphql?sdl
.PHONY : update-schema

# https://github.com/JosefPihrt/Roslynator/blob/master/docs/cli/README.md
analyze : ## Analyze with Roslynator
	dotnet tool run roslynator analyze
.PHONY : analyze

# https://www.jetbrains.com/help/resharper/ReSharper_Command_Line_Tools.html#install-and-use-resharper-command-line-tools-as-net-core-tools
inspect : ## Inspect with ReSharper, for example, `make OUTPUT=./inspection.xml inspect`
	dotnet tool run jb inspectcode --build ./Metabase.sln -o ${OUTPUT}
.PHONY : inspect

play : ## Run tests in `Metabase.Tests.Play` (semi-alternative to the buggy `dotnet script` REPL)
	dotnet test --filter "FullyQualifiedName~Metabase.Tests.Play"
.PHONY : play

# TODO Apply not only to `src` folder but ignore files and directories within .gitignore.
#      With git installed there are various options, which all have some
#      disadvantages, see
#      https://unix.stackexchange.com/questions/358270/find-files-that-are-not-in-gitignore
# Inspired by
# * https://stackoverflow.com/questions/45240387/how-can-i-remove-the-bom-from-a-utf-8-file/45240995#45240995
# * https://unix.stackexchange.com/questions/381230/how-can-i-remove-the-bom-from-a-utf-8-file/381263#381263
dos2unix : ## Strip the byte-order mark, also known as, BOM, and remove carriage returns
	find \
		. \
		\( -name "*.cs" -o -name "*.cshtml" \) \
		-type f \
		-exec sed -i -e "$(shell printf '1s/^\357\273\277//')" -e "s/\r//" {} +
	# find . -type f -exec dos2unix {} \;
.PHONY : dos2unix

format : ## Format code
	dotnet tool run jb cleanupcode --profile="Built-in: Reformat Code" ./Metabase.sln
.PHONY : format

syntax : ## Apply syntax style
	dotnet tool run jb cleanupcode --profile="Built-in: Reformat & Apply Syntax Style" ./Metabase.sln
.PHONY : syntax

# The profile `Built-in: Full Cleanup` does not update file headers.
# See https://www.jetbrains.com/help/resharper/Code_Cleanup__Index.html#profiles
cleanup : ## Clean-up code
	dotnet tool run jb cleanupcode ./Metabase.sln
.PHONY : cleanup

update-packages : ## Update packages
	for subproject in src test ; do ( \
		cd ./$${subproject} && \
		dotnet list package | \
			awk '/>/ {print $$2}' | \
			xargs -n 1 dotnet add package \
	) ; done
.PHONY : update-packages

update-tools : ## Update tools
	dotnet tool list | \
		tail -n +3 | \
		awk '{print $$1}' | \
		awk '!/dotnet-certificate-tool/' | \
		xargs -n 1 dotnet tool update
	dotnet tool run dotnet-sos install
.PHONY : update-tools

update : update-packages update-tools ## Update packages and tools
.PHONY : update

restore : VERBOSITY = normal
restore : ## Restore packages and tools with verbosity level `${VERBOSITY}` that defaults to `normal` (allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]), for example, `make VERBOSITY=detailed restore`
	dotnet restore --verbosity ${VERBOSITY}
.PHONY : restore

dotbuild : ## Build projects
	dotnet build
.PHONY : dotbuild

dedup : ## Dedeuplicate code lines matching the pattern `${PATTERN}`, for example, `make PATTERN="using Infrastructure.Aggregates" dedup`
	find . -name "*.cs" \
		| xargs -n 1 \
				gawk -i inplace "{ \
					if (/${PATTERN}/) \
						{ if (!seen[$$0]++) { print } } \
					else \
						{ print } \
					}"
.PHONY : dedup

add-migration : ## Add migration with name `${NAME}`, for example, `make NAME=InitialCreate add-migration`
	dotnet ef \
		--project src/Metabase.csproj \
		migrations \
		add \
		${NAME}
.PHONY : add-migration

migrate : ## Migrate database
	dotnet ef \
		--project src/Metabase.csproj \
		database \
		update
.PHONY : migrate

prepare-release : find-latest-migration generate-migration-script switch-migrations ## Prepare release
.PHONY : prepare-release

find-latest-migration : ## Store name of latest migration in `./src/Migrations/new`
	echo \
		$(notdir \
			$(basename \
				$(lastword \
					$(sort \
						$(shell find ./src/Migrations/ -regex ".*/[^/.]*_[^/.]*\.cs") \
					) \
				) \
			) \
		) \
	> ./src/Migrations/new
.PHONY : find-latest-migration

# https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#sql-scripts
generate-migration-script : FROM = $(file < ./src/Migrations/current)
generate-migration-script : TO = $(file < ./src/Migrations/new)
generate-migration-script : ## Generate migration and rollback scripts for upcoming release from the migration named by `./src/Migrations/current` to the one named by `./src/Migrations/new`
	dotnet ef \
		--project ./src/Metabase.csproj \
		migrations \
		script \
		${FROM} \
		${TO} \
		--output ./src/Migrations/migrate_from_${FROM}_to_${TO}.sql
	dotnet ef \
		--project ./src/Metabase.csproj \
		migrations \
		script \
		${TO} \
		${FROM} \
		--output ./src/Migrations/rollback_from_${FROM}_to_${TO}.sql
.PHONY : generate-migration-script

switch-migrations : ## Turn current into previous and new into current
	mv ./src/Migrations/current ./src/Migrations/previous
	mv ./src/Migrations/new ./src/Migrations/current
.PHONY : switch-migrations
