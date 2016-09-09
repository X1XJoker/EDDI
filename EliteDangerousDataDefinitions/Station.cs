﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteDangerousDataDefinitions
{
    /// <summary>
    /// A starport, outpost or port
    /// </summary>
    public class Station
    {
        /// <summary>The ID of this station in EDDB</summary>
        public long EDDBID { get; set; }

        /// <summary>The name</summary>
        public string name { get; set;  }

        /// <summary>The government</summary>
        public string government { get; set; }

        /// <summary>The faction</summary>
        public string faction { get; set; }

        /// <summary>The allegiance</summary>
        public string allegiance { get; set; }

        /// <summary>The state of the system</summary>
        public string state { get; set; }

        /// <summary>The economies of the station</summary>
        public List<string> economies { get; set; }

        /// <summary>How far this is from the star</summary>
        public long? distancefromstar { get; set; }

        /// <summary>The system in which this station resides</summary>
        public string systemname { get; set; }

        /// <summary>Does this station have refuel facilities?</summary>
        public bool? hasrefuel { get; set; }
        /// <summary>Does this station have rearm facilities?</summary>
        public bool? hasrearm { get; set; }
        /// <summary>Does this station have repair facilities?</summary>
        public bool? hasrepair { get; set; }
        /// <summary>Does this station have outfitting?</summary>
        public bool? hasoutfitting { get; set; }
        /// <summary>Does this station have a shipyard?</summary>
        public bool? hasshipyard { get; set; }
        /// <summary>Does this station have a market?</summary>
        public bool? hasmarket { get; set; }
        /// <summary>Does this station have a black market?</summary>
        public bool? hasblackmarket { get; set; }

        /// <summary>The model of the station</summary>
        public StationModel model { get; set; }

        /// <summary>What is the largest ship that can land here?</summary>
        public ShipSize largestpad { get; set;  }

        /// <summary>What commodities are bought/sold by the station</summary>
        public List<Commodity> commodities { get; set; }

        /// <summary>Is this station a starport?</summary>
        public bool IsStarport() { return model == StationModel.CoriolisStarport || model == StationModel.OcellusStarport || model == StationModel.OrbisStarport || model == StationModel.UnknownStarport; }

        /// <summary>Is this station an outpost?</summary>
        public bool IsOutpost() { return model == StationModel.CivilianOutpost|| model == StationModel.CommercialOutpost || model == StationModel.IndustrialOutpost || model == StationModel.MilitaryOutpost || model == StationModel.MiningOutpost || model == StationModel.ScientificOutpost || model == StationModel.UnsanctionedOutpost || model == StationModel.UnknownOutpost; }

        /// <summary>Is this station a planetary outpost?</summary>
        public bool IsPlanetaryOutpost() { return model == StationModel.PlanetaryOutpost; }

        /// <summary>Is this station a planetary  port?</summary>
        public bool IsPlanetaryPort() { return model == StationModel.PlanetaryPort; }

        /// <summary>Is this station planetary?</summary>
        public bool IsPlanetary() { return model == StationModel.PlanetaryOutpost || model == StationModel.PlanetaryPort || model == StationModel.UnknownPlanetary; }
    }

    public enum StationModel
    {
        CoriolisStarport,
        OcellusStarport,
        OrbisStarport,
        CivilianOutpost,
        CommercialOutpost,
        IndustrialOutpost,
        MilitaryOutpost,
        MiningOutpost,
        ScientificOutpost,
        UnsanctionedOutpost,
        PlanetaryOutpost,
        PlanetaryPort,
        PlanetarySettlement,
        UnknownStarport,
        UnknownOutpost,
        UnknownPlanetary,
        PlanetaryEngineerBase
    }
}
