using System;
using System.Collections.Generic;
using System.Database.Document;
using System.Database.Storage.Services;
using System.Database.Storage.Structures;
using System.Linq;
using LiteDB;

namespace System.Database.Query.Impl
{
    internal class QueryEquals : Query
    {
        private BsonValue _value;

        public QueryEquals(string field, BsonValue value)
            : base(field)
        {
            _value = value;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            var value = _value.Normalize(index.Options);
            var node = indexer.Find(index, value, false, Query.Ascending);

            if (node == null) yield break;

            yield return node;

            if (index.Options.Unique == false)
            {
                // navigate using next[0] do next node - if equals, returns
                while (!node.Next[0].IsEmpty && ((node = indexer.GetNode(node.Next[0])).Key.CompareTo(value) == 0))
                {
                    if (node.IsHeadTail(index)) yield break;

                    yield return node;
                }
            }
        }

        internal override void NormalizeValues(IndexOptions options)
        {
            _value = _value.Normalize(options);
        }

        internal override bool ExecuteFullScan(BsonDocument doc, IndexOptions options)
        {
            var val = doc.Get(this.Field).Normalize(options);

            return val.CompareTo(_value) == 0;
        }
    }
}