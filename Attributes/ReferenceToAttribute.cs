using System;

namespace Extend.MongoDbEntities.Attributes
{
    public class ReferenceToAttribute : Attribute
    {
        public string Key { get; }
        public ReferenceToAttribute(string key)
        {
            Key = key;
        }
    }
}
