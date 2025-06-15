using System;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "Player")]
    public class Player : IEquatable<Player>
    {
        [XmlElement(ElementName = "Name")]       public string  Name       { get; set; }
        [XmlElement(ElementName = "MemberNo")]   public int     MemberNo   { get; set; }
        [XmlElement(ElementName = "Substitute")] public int  Substitute { get; set; }
        [XmlElement(ElementName = "HAC")]        public decimal HAC        { get; set; }
        [XmlAttribute(AttributeName = "No")]     public int  No         { get; set; }

        public override string ToString() => Name;

        public string NameWithSubstitute => Substitute == 0 ? Name :  $"{Name} (S)";
        public string NameWithHac => $"{Name} ({HAC})";


        public override bool Equals(object obj)
        {
            if (obj is Player other)
                return MemberNo == other.MemberNo;

            return false;
        }

        public bool Equals(Player other)
        {
            return MemberNo == other.MemberNo;
        }

        public override int GetHashCode()
        {
            // Brug de samme properties som i Equals
            return HashCode.Combine(MemberNo);
        }
    }
}