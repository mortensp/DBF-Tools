
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using Baksteen.Extensions.DeepCopy;
using Caliburn.Micro;
using DBF.DataModel;
using Syncfusion.Data.Extensions;

namespace DBF.ViewModels
{
    public class ControlViewModel : Screen
    {
        static string path = @"C:\BC3\Hjemmeside\Resultater_2172\";
        //static string path = @"C:\BC3\Hjemmeside - Kopi\Resultater_2172\";
        //DbScope.IDbContextScope         scope;
        private readonly IWindowManager _windowManager;

    
        FileSystemWatcher   watcher;
        MainClub            mainClub;
        Club                selectedClub;
        PlayingTime         spilledag;
        List <Tournament>   tournaments;
        List <GroupSection> groupSection;

        JsonSerializerOptions JsonOptions = new JsonSerializerOptions { Converters = { new DecimalCommaConverter() } };
        Encoding              iso_8859_1  = System.Text.Encoding.GetEncoding("iso-8859-1");
        public Configuration Configuration { get; private set; }

        public ObservableCollection<Club>        Clubs      { get; set; } = [];
        public Club SelectedClub
        {
            get => selectedClub;
            set
            {
                spilledag = null;
                Set(ref selectedClub, value);
                HentSpilleDage();
            }
        }

        public ObservableCollection<PlayingTime> SpilleDage { get; set; } = [];
        public PlayingTime SpilleDag
        {
            get => spilledag;
            set
            {
                Pairs = new();
                Teams = new();
                Set(ref spilledag, value);
                HentSpilleDag();
            }
        }

        //public BindableCollection<BridgeTimerControl> Timers{ get; set; } = [];
        public BindableCollection<Pair>          Pairs      { get; set; } = [];
        public BindableCollection<Team>          Teams      { get; set; } = [];
        public BindableCollection<TimerSetting> TimerSettings => Configuration.TimerSettings;


        public ControlViewModel(IWindowManager windowManager, Configuration configuration)
        {
            Configuration = configuration;
            _windowManager                      = windowManager;
            //scope                               = DbScopeFactory.New();
            Thread.CurrentThread.CurrentCulture = Global.DkCulture;
            LoadMainClub();

        }

        #region Public Methods
        public void AddTimer()
        {
            foreach (var setting in TimerSettings)
                if (setting.Visibility != Visibility.Visible)
                {
                    setting.Visibility = Visibility.Visible;
                    Configuration.SaveSettings();
                    return;
                }
        }

        public void Help()
        {
        }

            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettingEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, int dwflags, IntPtr lParam);

            public async Task ShowProjector()
            {
                return;
                var screens = WpfScreenHelper.Screen.AllScreens.ToList();

                if (screens.Count >  1)
                {
                    var screenTwo = screens[1]; // Get the second screen (index 1)

                    // Create a new instance of the ViewModel for the window you want to open
                    var timersViewModel = IoC.Get<ProjectorViewModel>();

                    // Create dynamic setting for the window
                    dynamic setting = new ExpandoObject();

                    setting.Left        = screenTwo.WorkingArea.Left;
                    setting.Top         = screenTwo.WorkingArea.Top;
                    setting.Width       = screenTwo.WorkingArea.Width;
                    setting.Height      = screenTwo.WorkingArea.Height;
                    setting.WindowState = System.Windows.WindowState.Normal; // Sæt evt. til Maximized i Loaded-event
                    setting.View        = "Projektor";
                    await _windowManager.ShowWindowAsync(timersViewModel, null, setting);
                }
                else
                    MessageBox.Show("Der er ikke oprettet forbindelse til en sekundær skærm. Tast Win+K", "Info");
            }
        #endregion

        #region Private Method
            private void LoadMainClub()
            {
                mainClub = getMainClub();

                Clubs = mainClub.Clubs.OrderBy(c => c.Name)
                                      .ToObservableCollection();

                if (true)
                {
                    selectedClub = Clubs[0];
                    SpilleDage   = selectedClub.MainTournaments
                                               .SelectMany(mt => mt.PlayingTimes).OrderByDescending(s => s.Dato).ToObservableCollection();

                    var findDate = DateOnly.Parse("2025-03-05");

                    SpilleDag = SelectedClub.MainTournaments
                                            .SelectMany    (mt => mt.PlayingTimes)
                                            .FirstOrDefault(pt => DateOnly.Parse(pt.Date.Substring(0, 10)) == findDate);
                }
            }

            private void HentSpilleDage()
            {
                if (SelectedClub is null)
                    SpilleDage = mainClub.Clubs
                                         .SelectMany(club => club.MainTournaments)
                                         .SelectMany(mt => mt.PlayingTimes).OrderByDescending(s => s.Dato).ToObservableCollection();
                else
                    SpilleDage = SelectedClub.MainTournaments
                                             .SelectMany(mt => mt.PlayingTimes).OrderByDescending(s => s.Dato).ToObservableCollection();

                if (false)
                {
                    var findDate = DateOnly.Parse("2025-05-28");

                    SpilleDag = mainClub.Clubs
                                        .SelectMany    (club => club.MainTournaments)
                                        .SelectMany    (mt => mt.PlayingTimes)
                                        .FirstOrDefault(pt => DateOnly.Parse(pt.Date.Substring(0, 10)) == findDate);
                }
                else
                    SpilleDag = SpilleDage.Where(s => s.Dato >= DateTime.Now.AddHours(-5)).LastOrDefault() ?? SpilleDage.FirstOrDefault();
            }

            /// <summary>
            /// Henter XML data for den valgte Spille dag og klokkeslet
            /// </summary>
            private void HentSpilleDag()
            {
                tournaments  = getTournaments(spilledag);
                groupSection = getGroupSections(spilledag, tournaments);

                foreach (var grp in groupSection)
                    if (grp.Tournament.TournamentType.Text == "Parturnering")
                    {
                        if (grp.Resultlist is not null)
                            foreach (var pair in grp.Resultlist.Pairs)
                            {
                                pair.Group = grp.Tournament.Group;
                                Pairs.Add(pair.DeepCopy());
                            }

                        if (grp.Startlist is not null)
                            foreach (var pair in grp.Startlist.Pairs)
                            {
                                var res = Pairs.FirstOrDefault(p => p.Group == grp.Tournament.Group && p.PairNo == pair.PairNo);

                                if (res is null)
                                {
                                    pair.Group = grp.Tournament.Group;
                                    Pairs.Add(pair.DeepCopy());
                                }
                                else
                                    res.StartPos = pair.StartPos;
                            }
                    }
                    else
                    {
                        foreach (var team in grp.Rounds[0].Startlist.Teams)
                        {
                            team.Group = grp.Tournament.Group;
                            Teams.Add(team);
                        }

                        // Merge the four lists
                        foreach (var rnd in grp.Rounds)
                            foreach (var team in Teams.Where(t=>t.Group == grp.Tournament.Group))
                            {
                                var sta = rnd.Startlist.Teams.FirstOrDefault(t => t.TeamNo == team.TeamNo);
                                var res = rnd.Resultlist.Teams.FirstOrDefault(t => t.TeamNo == team.TeamNo);
                                var hac = rnd.HACResult.Teams.FirstOrDefault(t => t.TeamNo == team.TeamNo);
                                var but = rnd.ButlerResult.Teams.FirstOrDefault(t => t.TeamNo == team.TeamNo);
                                var oth = rnd.Resultlist.Teams.FirstOrDefault(t => t.TeamNo == res.OpponentTeamNo);
                                //
                                team.Merge(sta);
                                team.Merge(res);
                                team.Merge(hac);
                                team.Merge(but);
                            }
                    }
            }

            private MainClub getMainClub() => deserialize<MainClub>("Main.xml");

            private List<Tournament> getTournaments(PlayingTime pt)
            {
                List <Tournament> tournaments = new();

                if (pt.TournamentFiles is not null)
                    foreach (var tournamentFile in pt.TournamentFiles)
                    {
                        var tournament = deserialize<Tournament>(tournamentFile.Filename);

                        if (tournament != null)
                        {
                            tournament.SectionNo = tournamentFile.Section?.SectionNo ?? 1;
                            tournaments.Add(tournament);
                        }
                    }

                return tournaments;
            }

            private List<GroupSection> getGroupSections(PlayingTime pt, List<Tournament> tournaments)
            {
                List <GroupSection> sections = new();

                foreach (var tur in tournaments)
                {
                    var section = deserialize<GroupSection>(tur.SectionFile.FileName);

                    if (section != null)
                    {
                        section.Tournament = tur;
                        sections.Add(section);
                    }
                }

                return sections;
            }

            private void watchFile(string xmlFileName)
            {
                // Sæt stien til mappen og filnavnet du vil overvåge
                string directory = Path.GetDirectoryName(xmlFileName);
                string file      = Path.GetFileName(xmlFileName);

                watcher = new FileSystemWatcher(directory, file)
                          {
                              NotifyFilter = NotifyFilters.Attributes
                                           | NotifyFilters.CreationTime
                                           | NotifyFilters.DirectoryName
                                           | NotifyFilters.FileName
                                           | NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.Security
                                           | NotifyFilters.Size
                          };

                // Event når filen ændres
                watcher.Changed += (s, e) => Console.WriteLine($"Changed: {e.FullPath}");
                watcher.Created += (s, e) => Console.WriteLine($"Created: {e.FullPath}");
                watcher.Renamed += (s, e) => Console.WriteLine($"Renamed: {e.FullPath}");
                watcher.Deleted += (s, e) => Console.WriteLine($"Deleted: {e.FullPath}");

                // Start overvågning
                watcher.EnableRaisingEvents = true;
            }

            //private int OpenSecondScreen()
            //{
            //    var screen = WpfScreenHelper.Screen.PrimaryScreen;

            //    DEVMODE devMode      = new DEVMODE();
            //    devMode.dmSize       = (short)Marshal.SizeOf(typeof(DEVMODE));
            //    devMode.dmFields     = 0x00040000 | 0x00080000;  // DM_POSITION | DM_PELSWIDTH
            //    devMode.dmPositionX  = (int)screen.Bounds.Right; // Flyt til højre for primær skærm
            //    devMode.dmPositionY  = 0;
            //    devMode.dmPelsWidth  = 1920;
            //    devMode.dmPelsHeight = 1080;

            //    int result = ChangeDisplaySettingEx(null, ref devMode, IntPtr.Zero, 0, IntPtr.Zero);
            //    Debug.WriteLine(result == 0 ? "Skærm udvidet!" : "Fejl ved ændring af skærmopsætning.");
            //    return result;
            //}
            private T deserialize<T>(string fileName)
            {
                string fullPath = path + fileName;

                if (!File.Exists(fullPath))
                    return default;

                if (Path.GetExtension(fileName).ToLowerInvariant() == ".json")
                {
                    string json = File.ReadAllText(fullPath, iso_8859_1);
                    return JsonSerializer.Deserialize<T>(json, JsonOptions);
                }
                else // XML
                {
                    string xml = File.ReadAllText(fullPath, iso_8859_1);

                    // Erstat kun komma med punkt i decimaltal (fx 123,45 -> 123.45)
                    // Simpel version: erstatter alle komma mellem tal
                    xml = System.Text.RegularExpressions.Regex.Replace(xml, @"(?<=\d),(?=\d)", ".");

                    // Remove self-closing empty tags: <TagName/>
                    //xml = Regex.Replace(xml, @"<(\w+)(\s[^>]*)?/>\s*", string.Empty);
                    xml = Regex.Replace(xml, @"<\w+\s*(/>|/>\s*</\1*s>)", string.Empty);

                    // Remove empty tag pairs: <TagName></TagName>
                    //xml = Regex.Replace(xml, @"<(\w+)\s*([^>]*)?>\s*</\1>", string.Empty);
                    var       serializer = new XmlSerializer(typeof(T));
                    using var reader     = new StringReader(xml);
                    return (T)serializer.Deserialize(reader);
                }
            }
        #endregion
    }

    public class DecimalCommaConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return decimal.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("0.00", new CultureInfo("da-DK")));
        }
    }
}

