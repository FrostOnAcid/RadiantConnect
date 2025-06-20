﻿using System.Text.Json.Serialization;

namespace RadiantConnect.SocketServices.XMPP.DataTypes;
//ReSharper disable All
public record ValorantPresence(
    [property: JsonPropertyName("isValid")] bool? IsValid,
    [property: JsonPropertyName("sessionLoopState")] string SessionLoopState,
    [property: JsonPropertyName("partyOwnerSessionLoopState")] string PartyOwnerSessionLoopState,
    [property: JsonPropertyName("customGameName")] string CustomGameName,
    [property: JsonPropertyName("customGameTeam")] string CustomGameTeam,
    [property: JsonPropertyName("partyOwnerMatchMap")] string PartyOwnerMatchMap,
    [property: JsonPropertyName("partyOwnerMatchCurrentTeam")] string PartyOwnerMatchCurrentTeam,
    [property: JsonPropertyName("partyOwnerMatchScoreAllyTeam")] int? PartyOwnerMatchScoreAllyTeam,
    [property: JsonPropertyName("partyOwnerMatchScoreEnemyTeam")] int? PartyOwnerMatchScoreEnemyTeam,
    [property: JsonPropertyName("partyOwnerProvisioningFlow")] string PartyOwnerProvisioningFlow,
    [property: JsonPropertyName("provisioningFlow")] string ProvisioningFlow,
    [property: JsonPropertyName("matchMap")] string MatchMap,
    [property: JsonPropertyName("partyId")] string PartyId,
    [property: JsonPropertyName("isPartyOwner")] bool? IsPartyOwner,
    [property: JsonPropertyName("partyState")] string PartyState,
    [property: JsonPropertyName("partyAccessibility")] string PartyAccessibility,
    [property: JsonPropertyName("maxPartySize")] int? MaxPartySize,
    [property: JsonPropertyName("queueId")] string QueueId,
    [property: JsonPropertyName("partyLFM")] bool? PartyLFM,
    [property: JsonPropertyName("partyClientVersion")] string PartyClientVersion,
    [property: JsonPropertyName("partySize")] int? PartySize,
    [property: JsonPropertyName("tournamentId")] string TournamentId,
    [property: JsonPropertyName("rosterId")] string RosterId,
    [property: JsonPropertyName("partyVersion")] long? PartyVersion,
    [property: JsonPropertyName("queueEntryTime")] string QueueEntryTime,
    [property: JsonPropertyName("playerCardId")] string PlayerCardId,
    [property: JsonPropertyName("playerTitleId")] string PlayerTitleId,
    [property: JsonPropertyName("preferredLevelBorderId")] string PreferredLevelBorderId,
    [property: JsonPropertyName("accountLevel")] int? AccountLevel,
    [property: JsonPropertyName("competitiveTier")] int? CompetitiveTier,
    [property: JsonPropertyName("leaderboardPosition")] int? LeaderboardPosition,
    [property: JsonPropertyName("isIdle")] bool? IsIdle,
    [property: JsonPropertyName("tempValueX")] string TempValueX,
    [property: JsonPropertyName("tempValueY")] string TempValueY,
    [property: JsonPropertyName("tempValueZ")] bool? TempValueZ,
    [property: JsonPropertyName("tempValueW")] bool? TempValueW,
    [property: JsonPropertyName("tempValueV")] int? TempValueV,
    [property: JsonPropertyName("premierPresenceData")] PremierPresenceData PremierPresenceData
);

public record PremierPresenceData(
    [property: JsonPropertyName("rosterId")] string RosterId,
    [property: JsonPropertyName("rosterName")] string RosterName,
    [property: JsonPropertyName("rosterTag")] string RosterTag,
    [property: JsonPropertyName("division")] int? Division,
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("tempValueA")] int? TempValueA,
    [property: JsonPropertyName("tempValueB")] bool? TempValueB,
    [property: JsonPropertyName("tempValueC")] bool? TempValueC,
    [property: JsonPropertyName("tempValueD")] bool? TempValueD
);