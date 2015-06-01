﻿using System;
using System.Collections.Generic;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;
using System.Serialisation.Json.Schema;

#endif

namespace System.Serialisation.Json.Schema
{
    [Obsolete("JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
    internal class JsonSchemaModelBuilder
    {
        private JsonSchemaNodeCollection _nodes = new JsonSchemaNodeCollection();
        private Dictionary<JsonSchemaNode, JsonSchemaModel> _nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();
        private JsonSchemaNode _node;

        public JsonSchemaModel Build(JsonSchema schema)
        {
            _nodes = new JsonSchemaNodeCollection();
            _node = AddSchema(null, schema);

            _nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();
            JsonSchemaModel model = BuildNodeModel(_node);

            return model;
        }

        public JsonSchemaNode AddSchema(JsonSchemaNode existingNode, JsonSchema schema)
        {
            string newId;
            if (existingNode != null)
            {
                if (existingNode.Schemas.Contains(schema))
                    return existingNode;

                newId = JsonSchemaNode.GetId(existingNode.Schemas.Union(new[] { schema }));
            }
            else
            {
                newId = JsonSchemaNode.GetId(new[] { schema });
            }

            if (_nodes.Contains(newId))
                return _nodes[newId];

            JsonSchemaNode currentNode = (existingNode != null)
                ? existingNode.Combine(schema)
                : new JsonSchemaNode(schema);

            _nodes.Add(currentNode);

            AddProperties(schema.Properties, currentNode.Properties);

            AddProperties(schema.PatternProperties, currentNode.PatternProperties);

            if (schema.Items != null)
            {
                for (int i = 0; i < schema.Items.Count; i++)
                {
                    AddItem(currentNode, i, schema.Items[i]);
                }
            }

            if (schema.AdditionalItems != null)
                AddAdditionalItems(currentNode, schema.AdditionalItems);

            if (schema.AdditionalProperties != null)
                AddAdditionalProperties(currentNode, schema.AdditionalProperties);

            if (schema.Extends != null)
            {
                foreach (JsonSchema jsonSchema in schema.Extends)
                {
                    currentNode = AddSchema(currentNode, jsonSchema);
                }
            }

            return currentNode;
        }

        public void AddProperties(IDictionary<string, JsonSchema> source, IDictionary<string, JsonSchemaNode> target)
        {
            if (source != null)
            {
                foreach (KeyValuePair<string, JsonSchema> property in source)
                {
                    AddProperty(target, property.Key, property.Value);
                }
            }
        }

        public void AddProperty(IDictionary<string, JsonSchemaNode> target, string propertyName, JsonSchema schema)
        {
            JsonSchemaNode propertyNode;
            target.TryGetValue(propertyName, out propertyNode);

            target[propertyName] = AddSchema(propertyNode, schema);
        }

        public void AddItem(JsonSchemaNode parentNode, int index, JsonSchema schema)
        {
            JsonSchemaNode existingItemNode = (parentNode.Items.Count > index)
                ? parentNode.Items[index]
                : null;

            JsonSchemaNode newItemNode = AddSchema(existingItemNode, schema);

            if (!(parentNode.Items.Count > index))
            {
                parentNode.Items.Add(newItemNode);
            }
            else
            {
                parentNode.Items[index] = newItemNode;
            }
        }

        public void AddAdditionalProperties(JsonSchemaNode parentNode, JsonSchema schema)
        {
            parentNode.AdditionalProperties = AddSchema(parentNode.AdditionalProperties, schema);
        }

        public void AddAdditionalItems(JsonSchemaNode parentNode, JsonSchema schema)
        {
            parentNode.AdditionalItems = AddSchema(parentNode.AdditionalItems, schema);
        }

        private JsonSchemaModel BuildNodeModel(JsonSchemaNode node)
        {
            JsonSchemaModel model;
            if (_nodeModels.TryGetValue(node, out model))
                return model;

            model = JsonSchemaModel.Create(node.Schemas);
            _nodeModels[node] = model;

            foreach (KeyValuePair<string, JsonSchemaNode> property in node.Properties)
            {
                if (model.Properties == null)
                    model.Properties = new Dictionary<string, JsonSchemaModel>();

                model.Properties[property.Key] = BuildNodeModel(property.Value);
            }
            foreach (KeyValuePair<string, JsonSchemaNode> property in node.PatternProperties)
            {
                if (model.PatternProperties == null)
                    model.PatternProperties = new Dictionary<string, JsonSchemaModel>();

                model.PatternProperties[property.Key] = BuildNodeModel(property.Value);
            }
            foreach (JsonSchemaNode t in node.Items)
            {
                if (model.Items == null)
                    model.Items = new List<JsonSchemaModel>();

                model.Items.Add(BuildNodeModel(t));
            }
            if (node.AdditionalProperties != null)
                model.AdditionalProperties = BuildNodeModel(node.AdditionalProperties);
            if (node.AdditionalItems != null)
                model.AdditionalItems = BuildNodeModel(node.AdditionalItems);

            return model;
        }
    }
}