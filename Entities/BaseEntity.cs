using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using System;
using System.Runtime.Serialization;

namespace Extend.MongoDbEntities.Entities
{
    [DataContract]
    public class BaseEntity : IEntity, ICreatedOn, IModifiedOn
    {
        public string GenerateNewID()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        [DataMember(Name = "id")]
        [BsonId]
        [AsObjectId]
        public string ID { get; set; }

        [IgnoreDefault]
        [DataMember(Name = "createdAt")]
        [Field("createdAt")]
        public DateTime CreatedOn { get; set; }

        [IgnoreDefault]
        [DataMember(Name = "updatedAt")]
        [Field("updatedAt")]
        public DateTime ModifiedOn { get; set; }
    }
}
