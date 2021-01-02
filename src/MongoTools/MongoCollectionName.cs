using System;
using static System.String;

namespace MongoTools {
    /// <summary>
    /// Convention based mongo collection name.
    /// Returns a camelCase collection from any type while removing some forbidden suffixes:
    /// "Document", "Entity", "View", "Projection", "ProjectionDocument", "ProjectionEntity"
    /// </summary>
    public record MongoCollectionName {
        public static readonly MongoCollectionName Default = new("");

        readonly string _value;

        MongoCollectionName(string value) => _value = value;

        public static MongoCollectionName For<T>(string prefix = null) => For(typeof(T), prefix);

        public static MongoCollectionName For(Type type, string prefix = null) {
            var collectionName = type.Name;

            var suffixes = new[] {"Document", "Entity", "View", "Projection", "ProjectionDocument", "ProjectionEntity"};

            foreach (var suffix in suffixes) {
                if (collectionName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    collectionName = collectionName.Substring(0, collectionName.Length - suffix.Length);
            }

            if (!IsNullOrWhiteSpace(prefix)) collectionName = $"{prefix}-{collectionName}";

            return new MongoCollectionName(collectionName);
        }

        public override string ToString() => _value ?? "";

        public static implicit operator string(MongoCollectionName self) => self.ToString();
        public static implicit operator MongoCollectionName(string value) => IsNullOrWhiteSpace(value) ? Default : new MongoCollectionName(value);
    }
}
