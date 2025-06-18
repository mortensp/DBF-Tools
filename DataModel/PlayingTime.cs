using System;
using System.Xml.Serialization;

namespace DBF.DataModel
{
    [XmlRoot(ElementName = "PlayingTime")]
    public class PlayingTime : IEquatable<PlayingTime>
    {
        [XmlAttribute(AttributeName = "Date")] public string Date { get; set; }
        [XmlElement("GroupTournament")] public List<GroupTournament> TournamentFiles { get; set; }

        public DateTime Dato => DateTime.Parse(Date);

        public override string ToString() => string.IsNullOrEmpty(Date) ? null:Dato.ToShortDateString()+" "+Dato.ToShortTimeString();


        public override bool Equals(object obj)
        {
            if (obj is PlayingTime other)
                return Date == other.Date;

            return false;
        }

        public bool Equals(PlayingTime other)
        {
            return Date == other.Date;
        }

        public override int GetHashCode()
        {
            // Brug de samme properties som i Equals
            return HashCode.Combine(Date);
        }
    }
}