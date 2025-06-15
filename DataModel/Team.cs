using System;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Team")]
    public class Team : IEquatable<Team>
    {
                                                      public    string       Group              { get; set; }
        [XmlElement(ElementName = "Name")]            public    string       TeamName           { get; set; }
        [XmlElement(ElementName = "StartPos")]        public    string       StartPos           { get; set; }
        [XmlElement(ElementName = "AvgHAC")]          public    double       AvgHAC             { get; set; }
        [XmlElement(ElementName = "Player")]          public    List<Player> Players            { get; set; }
        [XmlAttribute(AttributeName = "No")]          public    int          TeamNo             { get; set; }
        [XmlElement(ElementName = "ImpScore")]        public    int          ImpScore           { get; set; }
        [XmlElement(ElementName = "ImpAdjustment")]   public    string       ImpAdjustment      { get; set; }
        [XmlElement(ElementName = "MatchPointScore")] public    string       MatchPointScore    { get; set; }
        [XmlElement(ElementName = "PenaltyScore")]    public    string       PenaltyScore       { get; set; }
        [XmlElement(ElementName = "EndScore")]        public    string       KP                 { get; set; }
        [XmlElement(ElementName = "OpponentTeamNo")]  public    int          OpponentTeamNo     { get; set; }

        [XmlElement(ElementName = "Half")]               public List<Half>   Halfs              { get; set; }
        [XmlElement(ElementName = "Tiebreak")]           public string       Tiebreak           { get; set; }
        [XmlElement(ElementName = "SectionTiebreak")]    public string       SectionTiebreak    { get; set; }
        [XmlElement(ElementName = "TournamentTiebreak")] public string       TournamentTiebreak { get; set; }
        //public decimal      KP                 { get; set; }

        // Håndtering af afvigende navne i XML
        [XmlElement(ElementName = "TeamName")]
        public string TeamName2
        {
            set
            {
                if (TeamName == null)
                    TeamName =  value; // For at undgå at slette det eksisterende navn, hvis det er sat
            }
        }

        // Dan et felt med alle navnene
        public string Names
        {
            get
            {
                var names = Players?.Select(p => p.NameWithSubstitute).ToList();
                return names != null ? string.Join(",  ", names) : string.Empty;
            }
        }

        public string NamesWithHac
        {
            get
            {
                var names = Players?.Select(p => p.NameWithHac).ToList();
                return names != null ? string.Join(",  ", names) : string.Empty;
            }
        }
        #region Overrides
        public override string ToString() => TeamName;

        public override bool Equals(object obj)
        {
            if (obj is Team other)
                return TeamNo == other.TeamNo;

            return false;
        }

        public bool Equals(Team other)
        {
            return TeamNo == other.TeamNo;
        }

        public override int GetHashCode()
        {
            // Brug de samme properties som i Equals
            return HashCode.Combine(TeamNo);
        }
        #endregion
    }
}
