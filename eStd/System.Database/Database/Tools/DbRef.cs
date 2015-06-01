using System;
using System.Database.Database;
using System.Database.Database.Collections;
using System.Database.Document;
using System.Linq;
using LiteDB;

namespace System.Database.Database.Tools
{
    /// <summary>
    /// Creates a field that is a reference for another document from another collection. T is another type
    /// </summary>
    public class DbRef<T>
        where T : new()
    {
        /// <summary>
        /// Used only for serialization/deserialize
        /// </summary>
        public DbRef()
        {
        }

        /// <summary>
        /// Initialize using reference collection name and collection Id
        /// </summary>
        public DbRef(string collection, BsonValue id)
        {
            if (string.IsNullOrEmpty(collection)) throw new ArgumentNullException("collection");
            if (id == null || id.IsNull || id.IsMinValue || id.IsMaxValue) throw new ArgumentNullException("id");

            this.Collection = collection;
            this.Id = id;
        }

        /// <summary>
        /// Initialize using reference collection name and collection Id
        /// </summary>
        public DbRef(LiteCollection<T> collection, BsonValue id)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (id == null || id.IsNull || id.IsMinValue || id.IsMaxValue) throw new ArgumentNullException("id");

            this.Collection = collection.Name;
            this._collection = collection;
            this.Id = id;
        }

        [BsonField("$ref")]
        public string Collection { get; set; }

        private LiteCollection<T> _collection;

        [BsonField("$id")]
        public BsonValue Id { get; set; }

        [BsonIgnore]
        public T Item { get; private set; }

        /// <summary>
        /// Fetch document reference return them. After fetch, you can use "Item" proerty do get ref document
        /// </summary>
        public T Fetch(LiteDatabase db)
        {
            if (this.Item == null)
            {
                this.Item = db.GetCollection<T>(this.Collection).FindById(this.Id);
            }

            return this.Item;
        }

        /// <summary>
        /// Fetch document reference return them. After fetch, you can use "Item" proerty do get ref document
        /// </summary>
        public T Fetch()
        {
            if (this.Item == null)
            {
                if(this._collection != null)
                    this.Item = this._collection.FindById(this.Id);
            }

            return this.Item;
        }
    }
}
