using System;
using System.Collections.Generic;

namespace DBF.BridgeMateModel
{
    public partial class Section
    {
        public short? Id { get; set; }
        public string? Letter { get; set; }
        public short? Tables { get; set; }
        public short? MissingPair { get; set; }
        public short? EwmoveBeforePlay { get; set; }
        public short? Session { get; set; }
        public short? ScoringType { get; set; }
        public int? Winners { get; set; }
        public string? OnlineEventGuid { get; set; }
        public short? OnlineEventRoundDuration { get; set; }
    }
}
