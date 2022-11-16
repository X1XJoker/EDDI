﻿using EddiDataDefinitions;
using EddiEddnResponder.Sender;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace EddiEddnResponder.Schemas
{
    [UsedImplicitly]
    public class OutfittingSchema : ISchema, ICapiSchema
    {
        public List<string> edTypes => new List<string> { "Outfitting" };

        // Track this so that we do not send duplicate data from the journal and from CAPI.
        private long? lastSentMarketID;

        public bool Handle(string edType, ref IDictionary<string, object> data, EDDNState eddnState)
        {
            if (edType is null || !edTypes.Contains(edType)) { return false; }
            if (data is null || eddnState?.GameVersion is null) { return false; }

            var marketID = JsonParsing.getLong(data, "MarketID");
            if (lastSentMarketID != marketID && data.TryGetValue("Items", out var modulesList))
            {
                // Only send the message if we have modules
                if (modulesList is List<object> modules && modules.Any())
                {
                    lastSentMarketID = marketID;

                    void UpdateKeyName(ref IDictionary<string, object> dataToUpdate, string oldKey, string newKey)
                    {
                        dataToUpdate[newKey] = dataToUpdate[oldKey];
                        dataToUpdate.Remove(oldKey);
                    }

                    UpdateKeyName(ref data, "StarSystem", "systemName");
                    UpdateKeyName(ref data, "StationName", "stationName");
                    UpdateKeyName(ref data, "MarketID", "marketId");
                    data.Remove("Items");
                    data.Add("modules", modules
                        .Select(m => JObject.FromObject(m)["Name"]?.ToString())
                        .Where(m => ApplyModuleNameFilter(m))
                        .Where(m => !Module.IsPowerPlay(m))
                        .ToList());

                    // Apply data augments
                    data = eddnState.GameVersion.AugmentVersion(data);

                    return true;
                }
            }

            return false;
        }

        public void Send(IDictionary<string, object> data)
        {
            EDDNSender.SendToEDDN("https://eddn.edcd.io/schemas/outfitting/2", data);
        }

        public IDictionary<string, object> Handle(JObject profileJson, JObject marketJson, JObject shipyardJson, JObject fleetCarrierJson, EDDNState eddnState, out bool handled)
        {
            handled = false;

            // Modules are included in shipyardJson
            if (shipyardJson?["modules"] is null || eddnState?.GameVersion is null) { return null; }

            var systemName = profileJson?["lastSystem"]?["name"]?.ToString();
            var stationName = shipyardJson["name"].ToString();
            var marketID = shipyardJson["id"].ToObject<long>();
            var timestamp = shipyardJson["timestamp"].ToObject<DateTime?>();

            // Sanity check - we must have a valid timestamp
            if (timestamp == null) { return null; }

            // Build our modules list
            var modules = shipyardJson["modules"].Children().Values()
                .Where(m => ApplyModuleSkuFilter(m))
                .Select(m => m["name"]?.ToString())
                .Where(m => ApplyModuleNameFilter(m))
                .ToList();

            // Continue if our modules list is not empty
            if (modules.Any())
            {
                lastSentMarketID = marketID;

                var data = new Dictionary<string, object>() as IDictionary<string, object>;
                data.Add("timestamp", Dates.FromDateTimeToString(timestamp));
                data.Add("systemName", systemName);
                data.Add("stationName", stationName);
                data.Add("marketId", marketID);
                data.Add("modules", modules);

                // Apply data augments
                data = eddnState.GameVersion.AugmentVersion(data, "CAPI-shipyard");

                handled = true;
                return data;
            }

            return null;
        }

        public void SendCapi(IDictionary<string, object> data)
        {
            EDDNSender.SendToEDDN("https://eddn.edcd.io/schemas/outfitting/2", data);
        }

        private bool ApplyModuleNameFilter(string m)
        {
            // Filter items that aren't weapons/utilities (Hpt_*), standard/internal modules (Int_*) or armour (*_Armour_*)
            // and the "Int_PlanetApproachSuite" module (for historical reasons)
            return (
                       m.StartsWith("Int_", StringComparison.InvariantCultureIgnoreCase) ||
                       m.StartsWith("Hpt_", StringComparison.InvariantCultureIgnoreCase) ||
                       m.Contains("_Armour_") || m.Contains("_armour_")
                   ) &&
                   m != "Int_PlanetApproachSuite";
        }

        private bool ApplyModuleSkuFilter(JToken m)
        {
            // Filter items that have a non-null "sku" property, unless it's "ELITE_HORIZONS_V_PLANETARY_LANDINGS" (i.e. PowerPlay and tech broker items).
            return m != null && (
                string.IsNullOrEmpty(m["sku"]?.ToString()) || 
                (m["sku"]?.ToString().Equals("ELITE_HORIZONS_V_PLANETARY_LANDINGS", StringComparison.InvariantCultureIgnoreCase) ?? false)
                );
        }
    }
}