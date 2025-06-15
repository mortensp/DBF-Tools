//using BridgeTypes;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Board")]
    public class Board :IEquatable<Board>
    {
        [XmlElement(ElementName = "Excluded")]         public   string  Excluded           { get; set; }
        [XmlElement(ElementName = "DatumScore")]       public   decimal DatumScore         { get; set; }
        [XmlElement(ElementName = "Declarer")]         public   string  Declarer           { get; set; }
        [XmlElement(ElementName = "Contract")]         public   string  Contract           { get; set; }
        [XmlElement(ElementName = "Doubling")]         public   string  Doubling           { get; set; }
        [XmlElement(ElementName = "Tricks")]           public   int     Tricks             { get; set; }
        [XmlElement(ElementName = "Lead")]             public   string  Lead               { get; set; }
        [XmlElement(ElementName = "Score")]            public   decimal Score              { get; set; }
        [XmlElement(ElementName = "PointsNS")]         public   decimal PointsNS           { get; set; }
        [XmlElement(ElementName = "PointsEW")]         public   decimal PointsEW           { get; set; }
        [XmlElement(ElementName = "PctNS")]            public   decimal PctNS              { get; set; }
        [XmlElement(ElementName = "PctEW")]            public   decimal PctEW              { get; set; }
        [XmlElement(ElementName = "AdjustmentStrNS")]  public   string  AdjustmentStrNS    { get; set; }
        [XmlElement(ElementName = "AdjustmentDescNS")] public   string  AdjustmentDescNS   { get; set; }
        [XmlElement(ElementName = "AdjustmentTypeNS")] public   string  AdjustmentTypeNS   { get; set; }
        [XmlElement(ElementName = "AdjustmentStrEW")]  public   string  AdjustmentStrEW    { get; set; }
        [XmlElement(ElementName = "AdjustmentDescEW")] public   string  AdjustmentDescEW   { get; set; }
        [XmlElement(ElementName = "AdjustmentTypeEW")] public   string  AdjustmentTypeEW   { get; set; }
        [XmlAttribute(AttributeName = "No")]           public   int  No                 { get; set; }
        [XmlElement(ElementName = "Dealer")]           public   string  Dealer             { get; set; }
        [XmlElement(ElementName = "Vulnerability")]    public   string  Vulnerability      { get; set; }
        [XmlElement(ElementName = "DDInfoValid")]      public   string  DDInfoValid        { get; set; }
        [XmlElement(ElementName = "ParContract")]      public   string  ParContract        { get; set; }
        [XmlElement(ElementName = "NorthHCP")]         public   int     NorthHCP           { get; set; }
        [XmlElement(ElementName = "SouthHCP")]         public   int     SouthHCP           { get; set; }
        [XmlElement(ElementName = "EastHCP")]          public   int     EastHCP            { get; set; }
        [XmlElement(ElementName = "WestHCP")]          public   int     WestHCP            { get; set; }
        [XmlElement(ElementName = "NNT")]              public   string  NNT                { get; set; }
        [XmlElement(ElementName = "NSP")]              public   string  NSP                { get; set; }
        [XmlElement(ElementName = "NHE")]              public   string  NHE                { get; set; }
        [XmlElement(ElementName = "NDI")]              public   string  NDI                { get; set; }
        [XmlElement(ElementName = "NCL")]              public   string  NCL                { get; set; }
        [XmlElement(ElementName = "SNT")]              public   string  SNT                { get; set; }
        [XmlElement(ElementName = "SSP")]              public   string  SSP                { get; set; }
        [XmlElement(ElementName = "SHE")]              public   string  SHE                { get; set; }
        [XmlElement(ElementName = "SDI")]              public   string  SDI                { get; set; }
        [XmlElement(ElementName = "SCL")]              public   string  SCL                { get; set; }
        [XmlElement(ElementName = "ENT")]              public   string  ENT                { get; set; }
        [XmlElement(ElementName = "ESP")]              public   string  ESP                { get; set; }
        [XmlElement(ElementName = "EHE")]              public   string  EHE                { get; set; }
        [XmlElement(ElementName = "EDI")]              public   string  EDI                { get; set; }
        [XmlElement(ElementName = "ECL")]              public   string  ECL                { get; set; }
        [XmlElement(ElementName = "WNT")]              public   string  WNT                { get; set; }
        [XmlElement(ElementName = "WSP")]              public   string  WSP                { get; set; }
        [XmlElement(ElementName = "WHE")]              public   string  WHE                { get; set; }
        [XmlElement(ElementName = "WDI")]              public   string  WDI                { get; set; }
        [XmlElement(ElementName = "WCL")]              public   string  WCL                { get; set; }
        [XmlElement(ElementName = "North")]            public   Cards   North              { get; set; }
        [XmlElement(ElementName = "East")]             public   Cards   East               { get; set; }
        [XmlElement(ElementName = "South")]            public   Cards   South              { get; set; }
        [XmlElement(ElementName = "West")]             public   Cards   West               { get; set; }

        //
        [XmlElement(ElementName = "IMPsNS")]             public string  IMPsNS             { get; set; }
        [XmlElement(ElementName = "IMPsEW")]             public string  IMPsEW             { get; set; }
        [XmlElement(ElementName = "IMPsNSAgainstDatum")] public string  IMPsNSAgainstDatum { get; set; }
        [XmlElement(ElementName = "IMPsEWAgainstDatum")] public string  IMPsEWAgainstDatum { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Board other)
                return No == other.No;

            return false;
        }

        public bool Equals(Board other)
        {
            return No == other.No;
        }

        public override int GetHashCode()
        {
            // Brug de samme properties som i Equals
            return HashCode.Combine(No);
        }
    }
}
