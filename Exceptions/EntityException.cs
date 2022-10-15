using System;

namespace Extend.MongoDbEntities.Exceptions
{
    public static class EntityException
    {
        public static void ThrowInvalidReferenceId(string referenceId)
        {
            throw new ArgumentNullException($"The reference Id {referenceId} invalid!");
        }

        public static void ThrowNotFoundReferenceId(string referenceId, Type referenceType = null)
        {
            throw new ArgumentNullException($"The reference Id {referenceId} not found in entity {referenceType?.Name}!");
        }
    }
}
