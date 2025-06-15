//using Azure.Core.Pipeline;
using System;
using System.Diagnostics;
using System.Media;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    //[DebuggerDisplay("{PairNo,2} {Players[0].TeamName} - {Players[1].TeamName}")]
    [XmlRoot(ElementName = "Pair")]
    public class Pair : IEquatable<Pair>
    {
                                                         public   string       Group                { get; set; }
        [XmlAttribute(AttributeName = "No")]             public   int          PairNo               { get; set; }
        [XmlElement(ElementName = "PairTournamentRank")] public   int          PairTournamentRank   { get; set; }
        [XmlElement(ElementName = "SectionRank")]        public   int          SectionRank          { get; set; }
        [XmlElement(ElementName = "TournamentRank")]     public   int          TournamentRank       { get; set; }
        [XmlElement(ElementName = "HACRankSection")]     public   int          HACRankSection       { get; set; }
        [XmlElement(ElementName = "AvgHAC")]             public   decimal      AvgHAC               { get; set; }
        [XmlElement(ElementName = "ExpectedScore")]      public   decimal      ExpectedScore        { get; set; }
        [XmlElement(ElementName = "StartPos")]           public   string       StartPos             { get; set; }
        [XmlElement(ElementName = "ActualScore")]        public   decimal      ActualScore          { get; set; }
        [XmlElement(ElementName = "Result")]             public   decimal      Result               { get; set; }
        [XmlAttribute(AttributeName = "Rank")]           public   int          Rank                 { get; set; }
        [XmlElement(ElementName = "MP")]                 public   string       MP                   { get; set; }
        [XmlElement(ElementName = "DeltaHAC")]           public   decimal      DeltaHAC             { get; set; }
        [XmlElement(ElementName = "Player")]             public   List<Player> Players              { get; set; }
        [XmlElement(ElementName = "TournamentScore")]    public   decimal      TournamentScore      { get; set; }
        [XmlElement(ElementName = "TournamentResult")]   public   decimal      TournamentResult     { get; set; }
        [XmlElement(ElementName = "HACTotal")]           public   decimal      HACTotal             { get; set; }
        [XmlElement(ElementName = "HACRankTotal")]       public   int          HACRankTotal         { get; set; }
        [XmlElement(ElementName = "Direction")]          public   string       Direction            { get; set; }

        [XmlElement(ElementName = "Comment")]    public           string       Comment              { get; set; }
        [XmlElement(ElementName = "TeamNumber")] public           string       TeamNumber           { get; set; }
        [XmlElement(ElementName = "TeamName")]   public           string       TeamName             { get; set; }

        [XmlElement(ElementName = "PairPoints")]           public string       PairPoints           { get; set; }
        [XmlElement(ElementName = "PairPct")]              public string       PairPct              { get; set; }
        [XmlElement(ElementName = "PairRank")]             public string       PairRank             { get; set; }
        [XmlElement(ElementName = "PairTournamentPoints")] public string       PairTournamentPoints { get; set; }
        [XmlElement(ElementName = "PairTournamentPct")]    public string       PairTournamentPct    { get; set; }

        //[XmlElement(ElementName = "NorthPlayer")]          public string       NorthPlayer          { get; set; }
        //[XmlElement(ElementName = "SouthPlayer")]          public string       SouthPlayer          { get; set; }
        //[XmlElement(ElementName = "EastPlayer")]           public string       EastPlayer           { get; set; }
        //[XmlElement(ElementName = "WestPlayer")]           public string       WestPlayer           { get; set; }
        [XmlAttribute(AttributeName = "Bye")] public              string       Bye                  { get; set; }

        // ---- 
        [XmlElement(ElementName = "PairResultIMPs")]   public     string       PairResultIMPs       { get; set; }
        [XmlElement(ElementName = "PairResultButler")] public     string       PairResultButler     { get; set; }
        [XmlElement(ElementName = "HACDelta")]         public     string       HACDelta             { get; set; }

        // Håndtering af afvigende navne i XML
        [XmlAttribute(AttributeName = "Direction")]
        public string Direction2
        {
            set
            {
                if (string.IsNullOrEmpty(Direction))
                    Direction = value
                    ; //todo: Empty Statement!! 
            }
        }

        // ---
  
        public string PairName
        {
            get
            {
                if (Players.Count == 0)
                    return "Oversiddere";

                if (Players.Count == 1)
                    return $"{Players[0].Name} - ?";

                if (Players.Count == 2)
                    return $"{Players[0].Name} - {Players[1].Name}";

                var names = Players[0].ToString();

                for (int i = 1; i <  Players.Count; i++)
                    names += $", {Players[i].Name}";

                return names;
            }
        }

        public override string ToString()
        {
            if (Players is null || Players.Count() == 0)
                return string.Empty;

            if (Players.Count() == 1)
                return $"{PairNo,2} {Players[0].Name} - ?";

            return $"{PairNo,2} {Players[0].Name} - {Players[1].Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Pair other)
                return PairNo == other.PairNo;

            return false;
        }

        public bool Equals(Pair other)
        {
            return PairNo == other.PairNo;
        }

        public override int GetHashCode()
        {
            // Brug de samme properties som i Equals
            return HashCode.Combine(PairNo);
        }
    }
}
