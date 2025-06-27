using System;
using System.Collections.Generic;

namespace DBF.BridgeMateModel
{
    public partial class PlayerNumber
    {
        public short? Section { get; set; }
        public short? Table { get; set; }
        public string? Direction { get; set; }
        public string? Number { get; set; }
        public string? Name { get; set; }
        public bool? Updated { get; set; }
        public DateTime? TimeLog { get; set; }
        public bool? Processed { get; set; }
        public short? Round { get; set; }
    }
}
