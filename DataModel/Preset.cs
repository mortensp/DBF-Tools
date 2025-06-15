using Caliburn.Micro;
//using RtfPipe.Tokens;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Data;

namespace DBF.DataModel
{
    public class Preset : PropertyChangedBase
    {
        private string name;
        private bool   customPreset;

        [JsonConstructorAttribute]
        public Preset( string name = null
                     , bool customPreset = true
                     , bool teamMatch = false
                     , int rounds = 0
                     , int boardsPerRound = 0
                     , int breakAfter = 0
                     , int hours = 0
                     , int minutes = 0
                     , int seconds = 0
                     , int transitionTime = 0
                     , int breakTime = 0
                     )
        {
            Name           = name?.ToString();
            CustomPreset        = customPreset;
            TeamMatch      = teamMatch;
            Rounds         = rounds;
            BoardsPerRound = boardsPerRound;
            BreakAfter     = breakAfter;
            Hours          = hours;
            Minutes        = minutes;
            Seconds        = seconds;
            TransitionTime = transitionTime;
            BreakTime      = breakTime;
        }

        public Preset(Preset other)
        {
            Name           = other.Name?.ToString();
            CustomPreset        = true;
            TeamMatch      = other.TeamMatch;
            Rounds         = other.Rounds;
            BoardsPerRound = other.BoardsPerRound;
            BreakAfter     = other.BreakAfter;
            Hours          = other.Hours;
            Minutes        = other.Minutes;
            Seconds        = other.Seconds;
            TransitionTime = other.TransitionTime;
            BreakTime      = other.BreakTime;
        }

                public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value?.ToString();
                    NotifyOfPropertyChange(nameof(Name));
                }
            }
        }

        public bool CustomPreset
        {
            get => customPreset;
            set
            {
                if (customPreset != value)
                {
                    customPreset = value;
                    NotifyOfPropertyChange(nameof(customPreset));
                }
            }
        }

        public bool TeamMatch      { get; set; }
        public int  Rounds         { get; set; }
        public int  BoardsPerRound { get; set; }
        public int  BreakAfter     { get; set; }
        public int  Hours          { get; set; }
        public int  Minutes        { get; set; }
        public int  Seconds        { get; set; }
        public int  TransitionTime { get; set; }
        public int  BreakTime      { get; set; }

   

        public bool Matches(Preset other)
        {
            return (other.Name is null || Name is null || Name==other.Name)
                && TeamMatch == other.TeamMatch
                 && Rounds == other.Rounds
                 && BoardsPerRound == other.BoardsPerRound
                 && BreakAfter == other.BreakAfter
                 && Hours == other.Hours
                 && Minutes == other.Minutes
                 && Seconds == other.Seconds
                 && TransitionTime == other.TransitionTime
                 && BreakTime == other.BreakTime;
        }

        override public string ToString() => Name;

        internal void Update(Preset other)
        {
            CustomPreset        = true;
            TeamMatch      = other.TeamMatch;
            Rounds         = other.Rounds;
            BoardsPerRound = other.BoardsPerRound;
            BreakAfter     = other.BreakAfter;
            Hours          = other.Hours;
            Minutes        = other.Minutes;
            Seconds        = other.Seconds;
            TransitionTime = other.TransitionTime;
            BreakTime      = other.BreakTime;             
        }
    }
}