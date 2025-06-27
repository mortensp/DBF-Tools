using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DBF.BridgeMateModel
{
 
    public partial class PlayerName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StrId { get; set; }

        public override string ToString() => $"{Id,6}: {Name}";
        
    }
}
