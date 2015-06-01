using System;
using System.Collections.Generic;
using System.Database.Document;
using System.Database.Storage.Services;
using System.Database.Storage.Structures;
using System.Linq;
using LiteDB;

namespace System.Database.Query.Impl
{
    /// <summary>
    /// Contains query do not work with index, only full scan
    /// </summary>
    internal class QueryContains : Query
    {
        private BsonValue _value;

        public QueryContains(string field, BsonValue value)
            : base(field)
        {
            _value = value;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            this.ExecuteMode = QueryExecuteMode.FullScan;

            return indexer.FindAll(index, Query.Ascending);
        }

        internal override void NormalizeValues(IndexOptions options)
        {
            _value = _value.Normalize(options);
        }

        internal override bool ExecuteFullScan(BsonDocument doc, IndexOptions options)
        {
            var val = doc.Get(this.Field).Normalize(options);

            if(!val.IsString) return false;

            return val.AsString.Contains(_value.AsString);
        }
    }
}