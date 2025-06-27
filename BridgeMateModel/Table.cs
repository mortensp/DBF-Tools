using System;
using System.Collections.Generic;

namespace DBF.BridgeMateModel
{
    public partial class Table
    {
        public short? Section { get; set; }
        public short? Table1 { get; set; }
        public short? ComputerId { get; set; }
        public short? Status { get; set; }
        public short? LogOnOff { get; set; }
        public short? CurrentRound { get; set; }
        public short? CurrentBoard { get; set; }
        public short? UpdateFromRound { get; set; }
        public short? Group { get; set; }
    }
}
