﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal sealed class TextJsonGeometryParamsJsonConverter : JsonConverter<GeometryParams>
    {
        public override GeometryParams Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException(string.Format(CultureInfo.CurrentCulture, RMResources.JsonUnexpectedToken));
            }

            using JsonDocument json = JsonDocument.ParseValue(ref reader);
            JsonElement root = json.RootElement;
            return TextJsonGeometryParamsJsonConverter.ReadProperty(root, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            GeometryParams geometryParams,
            JsonSerializerOptions options)
        {
            TextJsonGeometryParamsJsonConverter.WritePropertyValues(writer, geometryParams, options);
        }

        public static void WritePropertyValues(
            Utf8JsonWriter writer,
            GeometryParams geometryParams,
            JsonSerializerOptions options)
        {
            if (geometryParams == null)
            {
                return;
            }

            writer.WriteStartObject();

            if (geometryParams.Crs != null)
            {
                writer.WritePropertyName(JsonEncodedStrings.Crs);
                TextJsonCrsConverter.WritePropertyValues(writer, geometryParams.Crs, options);
            }

            if (geometryParams.BoundingBox != null)
            {
                writer.WritePropertyName(JsonEncodedStrings.BoundingBox);
                TextJsonBoundingBoxConverter.WritePropertyValues(writer, geometryParams.BoundingBox, options);
            }

            if (geometryParams.AdditionalProperties != null)
            {
                TextJsonObjectToPrimitiveConverter.SerializeDictionary(writer, geometryParams.AdditionalProperties, options);
            }

            writer.WriteEndObject();
        }

        public static GeometryParams ReadProperty(
            JsonElement root,
            JsonSerializerOptions options)
        {
            GeometryParams geometryParams = new GeometryParams();
            if (root.TryGetProperty(JsonEncodedStrings.Crs.EncodedUtf8Bytes, out JsonElement crsElement))
            {
                geometryParams.Crs = TextJsonCrsConverter.ReadProperty(crsElement);
            }

            if (root.TryGetProperty(JsonEncodedStrings.BoundingBox.EncodedUtf8Bytes, out JsonElement bboxElement))
            {
                geometryParams.BoundingBox = TextJsonBoundingBoxConverter.ReadProperty(bboxElement);
            }

            geometryParams.AdditionalProperties = new Dictionary<string, object>();

            // JsonExtensionData support
            foreach (JsonProperty jsonProperty in root.EnumerateObject())
            {
                if (jsonProperty.NameEquals(JsonEncodedStrings.Crs.EncodedUtf8Bytes)
                    || jsonProperty.NameEquals(JsonEncodedStrings.Type.EncodedUtf8Bytes)
                    || jsonProperty.NameEquals(JsonEncodedStrings.BoundingBox.EncodedUtf8Bytes)
                    || jsonProperty.NameEquals(JsonEncodedStrings.Coordinates.EncodedUtf8Bytes)
                    || jsonProperty.NameEquals(JsonEncodedStrings.Geometries.EncodedUtf8Bytes))
                {
                    continue;
                }

                geometryParams.AdditionalProperties.Add(jsonProperty.Name, TextJsonObjectToPrimitiveConverter.ReadProperty(jsonProperty.Value));
            }

            return geometryParams;
        }
    }
}