﻿using System.Collections.Generic;

namespace FaultyBot.Modules.Utility.Commands.Models
{
    public class MeasurementUnit
    {
        public List<string> Triggers { get; set; }
        public string UnitType { get; set; }
        public decimal Modifier { get; set; }
    }
}
