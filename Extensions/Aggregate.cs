using Extend.MongoDbEntities.Attributes;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extend.MongoDbEntities.Extensions
{
    public static class Aggregate
    {
        public static IAggregateFluent<T> Include<T>(this IAggregateFluent<T> source,
            params Expression<Func<T, object>>[] includes) where T : IEntity
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                Type type = typeof(T);

                if (!(include.Body is MemberExpression member))
                {
                    continue;
                }

                Type memberType = member.Type;
                PropertyInfo referenceProp = type.GetProperties()
                    .FirstOrDefault(t =>
                        t.PropertyType.Name == memberType.Name &&
                        t.GetCustomAttribute<ReferenceToAttribute>() != null);

                if (referenceProp == null)
                {
                    continue;
                }

                string foreignCollectionName = null;
                string localField = "_id";
                string foreignField = "_id";
                string asField = referenceProp.Name.ToLower();
                if (referenceProp.PropertyType.IsGenericType)
                {
                    source = ManyReference(source, memberType, type,
                        foreignCollectionName, localField, foreignField, asField);
                    continue;
                }

                source = OneReference(source, memberType, referenceProp,
                    foreignCollectionName, localField, foreignField, asField);
            }

            return source;
        }

        private static IAggregateFluent<T> OneReference<T>(IAggregateFluent<T> source,
            Type memberType, PropertyInfo referenceProp,
            string foreignCollectionName, string localField,
            string foreignField, string asField) where T : IEntity
        {
            foreignCollectionName =
                memberType.GetCustomAttribute<CollectionAttribute>()
                    ?.Name;

            localField = referenceProp
                .GetCustomAttribute<ReferenceToAttribute>()
                ?.Key;

            return source
                .Lookup(foreignCollectionName, localField, foreignField, asField)
                .Unwind(asField)
                .As<T>();
        }

        private static IAggregateFluent<T> ManyReference<T>(IAggregateFluent<T> source,
            Type memberType, Type type,
            string foreignCollectionName, string localField,
            string foreignField, string asField) where T : IEntity
        {
            Type foreignType = memberType.GenericTypeArguments[0];
            PropertyInfo foreignReference = foreignType.GetProperties()
                .FirstOrDefault(t =>
                    t.PropertyType.Name == type.Name &&
                    t.GetCustomAttribute<ReferenceToAttribute>() != null);

            foreignField = foreignReference
                ?.GetCustomAttribute<ReferenceToAttribute>()
                ?.Key;

            foreignCollectionName =
                 foreignType.GetCustomAttribute<CollectionAttribute>()
                     ?.Name;

            return source
                .Lookup(foreignCollectionName, localField, foreignField, asField)
                .As<T>();
        }
    }
}
