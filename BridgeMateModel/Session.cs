using System;
using System.Collections.Generic;

namespace DBF.BridgeMateModel
{
    public partial class Session
    {
        public short? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public string? Guid { get; set; }
        public short? Status { get; set; }
        public bool? ShowInApp { get; set; }
    }
}
