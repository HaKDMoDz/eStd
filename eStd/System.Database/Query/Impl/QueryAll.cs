using System;
using System.Collections.Generic;
using System.Database.Storage.Services;
using System.Database.Storage.Structures;
using System.Linq;
using LiteDB;

namespace System.Database.Query.Impl
{
    /// <summary>
    /// All is an Index Scan operation
    /// </summary>
    internal class QueryAll : Query
    {
        private int _order;

        public QueryAll(string field, int order)
            : base(field)
        {
            _order = order;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, _order);
        }

        internal override void NormalizeValues(IndexOptions options)
        {
        }

        internal override bool ExecuteFullScan(BsonDocument doc, IndexOptions options)
        {
            return true;
        }
    }
}