﻿using System.Text.Json;
using CyphersWatchfulEye.ValorantAPI.DataTypes;
using RadiantConnect.Methods;

namespace RadiantConnect.EventHandler.Events
{
    public class QueueEvents
    {
        internal enum PartyDataReturn
        {
            CustomGame,
            ChangeQueue
        }

        public delegate void QueueEvent<in T>(T value);

        public event QueueEvent<CustomGameData?>? OnCustomGameLobbyCreated;
        public event QueueEvent<string?>? OnQueueChanged;
        public event QueueEvent<string?>? OnEnteredQueue;
        public event QueueEvent<string?>? OnLeftQueue;

        private string GetEndpoint(string prefix, string log) => log.TryExtractSubstring("https", ']', startIndex => startIndex != -1, prefix);

        private async Task<T?> GetPartyData<T>(PartyDataReturn dataReturn, string endPoint) where T : class?
        {
            string? data = await Initiator.InternalSystem.Net.GetAsync(Initiator.InternalSystem.ClientData.GlzUrl, endPoint);

            return data is null ? null : dataReturn switch
            {
                PartyDataReturn.CustomGame => (T)Convert.ChangeType(JsonSerializer.Deserialize<PartyInfo>(data)?.CustomGameData, typeof(T))!,
                PartyDataReturn.ChangeQueue => (T)Convert.ChangeType(JsonSerializer.Deserialize<PartyInfo>(data)?.MatchmakingData.QueueID, typeof(T))!,
                _ => throw new ArgumentOutOfRangeException(nameof(dataReturn), dataReturn, null)
            };
        }

        public async void HandleQueueEvent(string invoker, string logData)
        {
            string parsedEndPoint = logData.Replace("/queue", "")
                                    .Replace("/matchmaking/join", "")
                                    .Replace("/matchmaking/leave", "")
                                    .Replace("/makecustomgame", "");
            if (!logData.Contains("https")) return;
            
            switch (invoker)
            {
                case "Party_ChangeQueue":
                    OnQueueChanged?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(Initiator.InternalSystem.ClientData.GlzUrl, parsedEndPoint)));
                    break;
                case "Party_EnterMatchmakingQueue":
                    OnEnteredQueue?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(Initiator.InternalSystem.ClientData.GlzUrl, parsedEndPoint)));
                    break;
                case "Party_LeaveMatchmakingQueue":
                    OnLeftQueue?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(Initiator.InternalSystem.ClientData.GlzUrl, parsedEndPoint)));
                    break;
                case "Party_MakePartyIntoCustomGame":
                    OnCustomGameLobbyCreated?.Invoke(await GetPartyData<CustomGameData>(PartyDataReturn.CustomGame, GetEndpoint(Initiator.InternalSystem.ClientData.GlzUrl, parsedEndPoint)));
                    break;
            }
        }
    }
}
