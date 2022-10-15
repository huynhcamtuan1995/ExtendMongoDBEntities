using Extend.MongoDbEntities.Attributes;
using Extend.MongoDbEntities.Entities;
using Extend.MongoDbEntities.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extend.MongoDbEntities.Context
{
    public class ExtendContext : DBContext
    {
        public ExtendContext(string database, MongoClientSettings mongoClientSettings) : base(database, mongoClientSettings)
        {
        }

        protected override Action<T> OnBeforeSave<T>()
        {
            void Action(T item)
            {
                List<PropertyInfo> props = item.GetType().GetProperties().ToList();
                IEnumerable<PropertyInfo> referenceProps = props
                    .Where(t => t.GetCustomAttribute<ReferenceToAttribute>() != null);

                foreach (PropertyInfo referenceProp in referenceProps)
                {
                    ReferenceToAttribute attribute =
                        referenceProp.GetCustomAttribute<ReferenceToAttribute>();

                    PropertyInfo referenceKey =
                        props.Find(x => x.Name == attribute.Key);

                    string referenceId = (string)referenceKey?.GetValue(item, null);

                    ValidateReferenceId(referenceId, referenceProp.PropertyType);
                }
            }

            return Action;
        }

        protected override Action<UpdateBase<T>> OnBeforeUpdate<T>()
        {
            void Action(UpdateBase<T> update)
            {
                //update.AddModification(f => f.UpdatedBy, "SomeOne");
                //update.AddModification(f => f.UpdateDate, DateTime.UtcNow);
            }

            return Action;
        }

        private void ValidateReferenceId(string referenceId, Type referenceType)
        {
            if (string.IsNullOrEmpty(referenceId) ||
                ObjectId.TryParse(referenceId, out _) == false)
            {
                EntityException.ThrowInvalidReferenceId(referenceId);
            }

            MethodInfo methodInfo = typeof(DB).GetMethod("Queryable");
            MethodInfo genericMethod = methodInfo?.MakeGenericMethod(referenceType);
            IMongoQueryable<BaseEntity> queryable =
                (IMongoQueryable<BaseEntity>)genericMethod?.Invoke(null, new object[] { null, null });

            BaseEntity entity = queryable?.FirstOrDefault(t => t.ID == referenceId);
            if (entity == null)
            {
                EntityException.ThrowNotFoundReferenceId(referenceId);
            }
        }
    }
}
