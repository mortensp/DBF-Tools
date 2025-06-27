using System;
using System.Collections.Generic;

namespace DBF.BridgeMateModel
{
    public partial class RoundDatum
    {
        public short? Section { get; set; }
        public short? Table { get; set; }
        public short? Round { get; set; }
        public short? Nspair { get; set; }
        public short? Ewpair { get; set; }
        public short? LowBoard { get; set; }
        public short? HighBoard { get; set; }
        public string? CustomBoards { get; set; }
    }
}
