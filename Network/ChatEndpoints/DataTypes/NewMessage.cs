﻿using System.Text.Json.Serialization;
namespace RadiantConnect.Network.ChatEndpoints.DataTypes;

// ReSharper disable All

public record NewMessage(
    [property: JsonPropertyName("cid")] string ConversationId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("type")] string ChatType
);