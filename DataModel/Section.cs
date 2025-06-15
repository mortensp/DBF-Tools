using DBF.DataModel;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Section")]
    public class Section
    {
        [XmlElement(ElementName= "Test")] public string Test { get; set; }
        [XmlAttribute(AttributeName = "Id")]      public string Id        { get; set; }

        [XmlAttribute(AttributeName = "SectionNo")] public int    SectionNo { get; set; }

        // Håndtering af afvigende navne i XML
        [XmlAttribute(AttributeName = "Section")]
        public int SectionNo2
        {
            set
            {
                SectionNo = value;
            }
        }

        //
        //
    }
}