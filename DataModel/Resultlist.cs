using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Resultlist")]
    public class Resultlist
    {
        [XmlElement(ElementName = "ScoreTransferred")]  public decimal    ScoreTransferred  { get; set; }
        [XmlElement(ElementName = "Pair")]              public List<Pair> Pairs             { get; set; }
        [XmlElement(ElementName = "ScoreRegulated")]    public decimal    ScoreRegulated    { get; set; }
        [XmlElement(ElementName = "SectionResultNote")] public string     SectionResultNote { get; set; }
        //
        [XmlElement(ElementName = "Team")] public              List<Team> Teams              { get; set; }
    }
}
