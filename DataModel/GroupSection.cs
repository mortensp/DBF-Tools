using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Section")]
    public class GroupSection
    {
        [XmlElement(ElementName = "Date")]                  public string      Date              { get; set; }
        [XmlElement(ElementName = "Start")]                 public string      Start             { get; set; }
        [XmlElement(ElementName = "End")]                   public string      End               { get; set; }
        [XmlElement(ElementName = "MeanScore")]             public decimal     MeanScore         { get; set; }
        [XmlElement(ElementName = "AvgHAC")]                public decimal     AvgHAC            { get; set; }
        [XmlElement(ElementName = "Startlist")]             public Startlist   Startlist         { get; set; }
        [XmlElement(ElementName = "Resultlist")]            public Resultlist  Resultlist        { get; set; }
        [XmlElement(ElementName = "Round")]                 public List<Round> Rounds            { get; set; }
        [XmlElement(ElementName = "Boards")]                public Boards      Boards            { get; set; }
        [XmlAttribute(AttributeName = "No")]                public string      No                { get; set; }
        [XmlAttribute(AttributeName = "MainTournamentId")]  public string      MainTournamentId  { get; set; }
        [XmlAttribute(AttributeName = "GroupTournamentId")] public string      GroupTournamentId { get; set; }

        [XmlAttribute(AttributeName = "HacRoundBOId")] public string HacRoundBOId { get; set; }

        public int BoardsPerRound => Boards.Boardspec.Boards.Count / (Rounds?.Count ?? 1);

        public                                                     Tournament  Tournament        { get; set; }


    }
}
