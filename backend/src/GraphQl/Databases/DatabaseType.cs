using System;
using System.Linq.Expressions;
using HotChocolate.Types;
using Metabase.Data;
using Metabase.GraphQl.DataX;
using Metabase.GraphQl.Users;

namespace Metabase.GraphQl.Databases;

public sealed class DatabaseType
    : EntityType<Database, DatabaseByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<Database> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(t => t.Operator)
            .Type<NonNullType<ObjectType<DatabaseOperatorEdge>>>()
            .Resolve(context =>
                new DatabaseOperatorEdge(
                    context.Parent<Database>()
                )
            );
        descriptor
            .Field(t => t.OperatorId).Ignore();
        ConfigureDataField(
            descriptor,
            "data",
            _ => _.GetDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<DataPropositionInput>(
            descriptor,
            "allData",
            _ => _.GetAllDataAsync(default!, default, default, default, default, default, default, default,
                default!, default!, default)
        );
        ConfigureHasDataField<DataPropositionInput>(
            descriptor,
            "hasData",
            _ => _.GetHasDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureDataField(
            descriptor,
            "opticalData",
            _ => _.GetOpticalDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<OpticalDataPropositionInput>(
            descriptor,
            "allOpticalData",
            _ => _.GetAllOpticalDataAsync(default!, default, default, default, default, default, default, default,
                default!, default!, default)
        );
        ConfigureHasDataField<OpticalDataPropositionInput>(
            descriptor,
            "hasOpticalData",
            _ => _.GetHasOpticalDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureDataField(
            descriptor,
            "hygrothermalData",
            _ => _.GetHygrothermalDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<HygrothermalDataPropositionInput>(
            descriptor,
            "allHygrothermalData",
            _ => _.GetAllHygrothermalDataAsync(default!, default, default, default, default, default, default,
                default, default!, default!, default)
        );
        ConfigureHasDataField<HygrothermalDataPropositionInput>(
            descriptor,
            "hasHygrothermalData",
            _ => _.GetHasHygrothermalDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureDataField(
            descriptor,
            "calorimetricData",
            _ => _.GetCalorimetricDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<CalorimetricDataPropositionInput>(
            descriptor,
            "allCalorimetricData",
            _ => _.GetAllCalorimetricDataAsync(default!, default, default, default, default, default, default,
                default, default!, default!, default)
        );
        ConfigureHasDataField<CalorimetricDataPropositionInput>(
            descriptor,
            "hasCalorimetricData",
            _ => _.GetHasCalorimetricDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureDataField(
            descriptor,
            "photovoltaicData",
            _ => _.GetPhotovoltaicDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<PhotovoltaicDataPropositionInput>(
            descriptor,
            "allPhotovoltaicData",
            _ => _.GetAllPhotovoltaicDataAsync(default!, default, default, default, default, default, default,
                default, default!, default!, default)
        );
        ConfigureHasDataField<PhotovoltaicDataPropositionInput>(
            descriptor,
            "hasPhotovoltaicData",
            _ => _.GetHasPhotovoltaicDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureDataField(
            descriptor,
            "geometricData",
            _ => _.GetGeometricDataAsync(default!, default, default, default, default!, default!, default)
        );
        ConfigureAllDataField<GeometricDataPropositionInput>(
            descriptor,
            "allGeometricData",
            _ => _.GetAllGeometricDataAsync(default!, default, default, default, default, default, default,
                default, default!, default!, default)
        );
        ConfigureHasDataField<GeometricDataPropositionInput>(
            descriptor,
            "hasGeometricData",
            _ => _.GetHasGeometricDataAsync(default!, default, default, default, default!, default!, default)
        );
        descriptor
            .Field("canCurrentUserUpdateNode")
            .ResolveWith<DatabaseResolvers>(x =>
                x.GetCanCurrentUserUpdateNodeAsync(default!, default!, default!, default!, default!))
            .UseUserManager();
        descriptor
            .Field("canCurrentUserVerifyNode")
            .ResolveWith<DatabaseResolvers>(x =>
                x.GetCanCurrentUserVerifyNodeAsync(default!, default!, default!, default!, default!))
            .UseUserManager();
    }

    private static void ConfigureDataField(
        IObjectTypeDescriptor<Database> descriptor,
        string fieldName,
        Expression<Func<DatabaseResolvers, object?>> resolverMethod
    )
    {
        descriptor
            .Field(fieldName)
            .Argument("id", _ => _.Type<NonNullType<UuidType>>())
            .Argument("timestamp", _ => _.Type<DateTimeType>())
            .Argument("locale", _ => _.Type<StringType>())
            .ResolveWith(resolverMethod);
    }

    private static void ConfigureAllDataField<TDataPropositionInput>(
        IObjectTypeDescriptor<Database> descriptor,
        string fieldName,
        Expression<Func<DatabaseResolvers, object?>> resolverMethod
    )
    {
        descriptor
            .Field(fieldName)
            .Argument("where", _ => _.Type<InputObjectType<TDataPropositionInput>>())
            .Argument("timestamp", _ => _.Type<DateTimeType>())
            .Argument("locale", _ => _.Type<StringType>())
            .Argument("first", _ => _.Type<NonNegativeIntType>())
            .Argument("after", _ => _.Type<StringType>())
            .Argument("last", _ => _.Type<NonNegativeIntType>())
            .Argument("before", _ => _.Type<StringType>())
            .ResolveWith(resolverMethod);
    }

    private static void ConfigureHasDataField<TDataPropositionInput>(
        IObjectTypeDescriptor<Database> descriptor,
        string fieldName,
        Expression<Func<DatabaseResolvers, object?>> resolverMethod
    )
    {
        descriptor
            .Field(fieldName)
            .Argument("where", _ => _.Type<InputObjectType<TDataPropositionInput>>())
            .Argument("timestamp", _ => _.Type<DateTimeType>())
            .Argument("locale", _ => _.Type<StringType>())
            .ResolveWith(resolverMethod);
    }
}