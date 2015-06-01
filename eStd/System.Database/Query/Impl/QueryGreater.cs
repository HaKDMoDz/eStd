using System;
using System.Collections.Generic;
using System.Database.Document;
using System.Database.Storage.Services;
using System.Database.Storage.Structures;
using System.Linq;
using LiteDB;

namespace System.Database.Query.Impl
{
    internal class QueryGreater : Query
    {
        private BsonValue _value;
        private bool _equals;

        public QueryGreater(string field, BsonValue value, bool equals)
            : base(field)
        {
            _value = value;
            _equals = equals;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            // find first indexNode
            var value = _value.Normalize(index.Options);
            var node = indexer.Find(index, value, true, Query.Ascending);

            if (node == null) yield break;

            // move until next is last
            while (node != null)
            {
                var diff = node.Key.CompareTo(value);

                if (diff == 1 || (_equals && diff == 0))
                {
                    if (node.IsHeadTail(index)) yield break;

                    yield return node;
                }

                node = indexer.GetNode(node.Next[0]);
            }
        }

        internal override void NormalizeValues(IndexOptions options)
        {
            _value = _value.Normalize(options);
        }

        internal override bool ExecuteFullScan(BsonDocument doc, IndexOptions options)
        {
            var val = doc.Get(this.Field).Normalize(options);

            return val.CompareTo(_value) >= (_equals ? 0 : 1);
        }
    }
}