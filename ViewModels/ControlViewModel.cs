using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Baksteen.Extensions.DeepCopy;
using Caliburn.Micro;
using DBF.DataModel;
using DBF.UserControls;
using DBF.Views;
using Microsoft.DotNet.DesignTools.ViewModels;
using Syncfusion.Data.Extensions;

namespace DBF.ViewModels
{
    public class ControlViewModel : Screen
    {
        static string path = @"C:\BC3\Hjemmeside\Resultater_2172\";
        //static string path = @"C:\BC3\Hjemmeside - Kopi\Resultater_2172\";
        //DbScope.IDbContextScope         scope;
        private readonly IWindowManager _windowManager;

        FileSystemWatcher     watcher;
        MainClub              mainClub;
        Club                  selectedClub;
        PlayingTime           spilledag;
        List <Tournament>     tournaments;
        List <GroupSection>   groupSection;
        JsonSerializerOptions JsonOptions = new JsonSerializerOptions { Converters = { new DecimalCommaConverter() } };
        Encoding              iso_8859_1  = System.Text.Encoding.GetEncoding("iso-8859-1");

        #region Constructors
            public ControlViewModel(IWindowManager windowManager, Configuration configuration)
            {
                Configuration                       = configuration;
                _windowManager                      = windowManager;
                Thread.CurrentThread.CurrentCulture = Global.DkCulture;
                LoadMainClub();
            }
        #endregion

        #region Public Properties
            private UserControl startListControl = new StartListControl();
            private UserControl timersPanel      = new TimersPanel() { ButtonsVisibility = Visibility.Collapsed };
            private UserControl resultsControl   = new ResultsControl();
            private UserControl _currentView;

            public UserControl CurrentView
            {
                get => _currentView ?? (_currentView = timersPanel);
                set
                {
                    _currentView = value;
                    NotifyOfPropertyChange(() => CurrentView);
                }
            }

            public Configuration                     Configuration { get; set; }

            public ObservableCollection<Club>        Clubs         { get; set; } = [];
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

            public ObservableCollection<PlayingTime> SpilleDage    { get; set; } = [];
            public PlayingTime SpilleDag
            {
                get => spilledag;
                set
                {
                    if (Set(ref spilledag, value))
                    {
                        Pairs = new();
                        Teams = new();
                        HentSpilleDag();
                    }
                }
            }

            public BindableCollection<Pair>          Pairs         { get; set; } = [];
            public BindableCollection<Team>          Teams         { get; set; } = [];
        #endregion

        #region Public Methods
            public void AddTimer()
            {
                Configuration.AddTimer();
                //foreach (var setting in BridgeTimers)
                //    if (setting.Visibility != Visibility.Visible)
                //    {
                //        setting.Visibility = Visibility.Visible;
                //        Configuration.SaveSettings();
                //        return;
                //    }
            }

            public void Help()
            {
            }

            public async void ShowStartList()
            {
                if (CurrentView is not StartListControl)
                    CurrentView = startListControl;

                await ShowProjector();
                //bringShellViewToFront();
            }

            public async void ShowBridgeTimers()
            {
                if (CurrentView is not TimersPanel)
                    CurrentView = timersPanel;

                await ShowProjector();
                //bringShellViewToFront();
            }

            public async void ShowResults()
            {
                if (CurrentView is not ResultsControl)
                    CurrentView = resultsControl;

                await ShowProjector();
                //bringShellViewToFront();
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
                            foreach (var team in Teams.Where(t => t.Group == grp.Tournament.Group))
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

                NotifyOfPropertyChange(nameof(Pairs));
                NotifyOfPropertyChange(nameof(Teams));
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

            //private void watchFile(string xmlFileName)
            //{
            //    // Sæt stien til mappen og filnavnet du vil overvåge
            //    string directory = Path.GetDirectoryName(xmlFileName);
            //    string file = Path.GetFileName(xmlFileName);

            //    watcher = new FileSystemWatcher(directory, file)
            //    {
            //        NotifyFilter = NotifyFilters.Attributes
            //                               | NotifyFilters.CreationTime
            //                               | NotifyFilters.DirectoryName
            //                               | NotifyFilters.FileName
            //                               | NotifyFilters.LastAccess
            //                               | NotifyFilters.LastWrite
            //                               | NotifyFilters.Security
            //                               | NotifyFilters.Size
            //    };

            //    // Event når filen ændres
            //    watcher.Changed += (s, e) => Console.WriteLine($"Changed: {e.FullPath}");
            //    watcher.Created += (s, e) => Console.WriteLine($"Created: {e.FullPath}");
            //    watcher.Renamed += (s, e) => Console.WriteLine($"Renamed: {e.FullPath}");
            //    watcher.Deleted += (s, e) => Console.WriteLine($"Deleted: {e.FullPath}");

            //    // Start overvågning
            //    watcher.EnableRaisingEvents = true;
            //}

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

                    xml = Regex.Replace(xml, @">(-|\s)+<", "><");

                    // Erstat kun komma med punkt i decimaltal (fx 123,45 -> 123.45)
                    // Simpel version: erstatter alle komma mellem tal
                    xml = System.Text.RegularExpressions.Regex.Replace(xml, @"(?<=\d),(?=\d)", ".");

                    // Remove empty tags: <TagName></TagName> and <TagName/>
                    xml = Regex.Replace(xml, @"<(\w+)(\s[^>]*)?>\s*</\1>", string.Empty);
                    xml = Regex.Replace(xml, @"<\w+\s*(/>|/>\s*</\1*s>)" , string.Empty);

                    // Remove empty tag pairs: <TagName></TagName>
                    //xml = Regex.Replace(xml, @"<(\w+)\s*([^>]*)?>\s*</\1>", string.Empty);
                    var       serializer = new XmlSerializer(typeof(T));
                    using var reader     = new StringReader(xml);
                    return (T)serializer.Deserialize(reader);
                }
            }

            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettingEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, int dwflags, IntPtr lParam);

            private async Task ShowProjector()
            {
                var screens = WpfScreenHelper.Screen.AllScreens.ToList();

                if (screens.Count == 0)
                {
                    MessageBox.Show("Der er ikke oprettet forbindelse til en sekundær skærm. Tast Win+K", "Info");
                }
                else
                {
                    var screenTwo = screens[^1]; // Get the second screen (index 1)

                    var projectorView = Application.Current.Windows.OfType<ProjectorView>().FirstOrDefault();

                    if (projectorView is null)
                    {
                        await _windowManager.ShowWindowAsync(this, "ProjectorView");
                        projectorView = Application.Current.Windows.OfType<ProjectorView>().FirstOrDefault();
                    }

                    projectorView.WindowStartupLocation = WindowStartupLocation.Manual;
                    projectorView.Left                  = screenTwo.WorkingArea.Left;
                    projectorView.Top                   = screenTwo.WorkingArea.Top;
                    projectorView.Width                 = screenTwo.WorkingArea.Width;
                    projectorView.Height                = screenTwo.WorkingArea.Height;
                    projectorView.WindowState           = WindowState.Maximized;
                    //projectorView.Activate();
                }

                // Flyt ShellView-aktivering herud, så den ALTID får fokus til sidst
                var shellVm   = IoC.Get<ShellViewModel>();
                var shellView = shellVm.GetView() as Window;

                if (shellView != null)
                {
                    shellView.Activate();
                    shellView.Topmost = true;                    
                    shellView.Topmost = false;
                    shellView.Focus();                
                }
            }

            //private void bringViewToFront(string vm = null)
            //{
            //    var shellView = Application.Current.Windows
            //                                       .OfType<DBF.Views.ShellView>()
            //                                       .FirstOrDefault();

            //    if (shellView != null)
            //    {
            //        if (shellView.WindowState == WindowState.Minimized)
            //            shellView.WindowState =  WindowState.Normal;

            //        shellView.Activate();
            //        shellView.Topmost = true;
            //        shellView.Topmost = false;
            //        shellView.Focus();
            //    }
            //}

            //private void bringShellViewToFront()
            //{
            //    var shellVm   = IoC.Get<ShellViewModel>();
            //    var shellView = shellVm.GetView() as Window;

            //    if (shellView != null)
            //    {
            //        shellView.Activate();
            //        shellView.Topmost = true;
            //        shellView.Focus();
            //        shellView.Topmost = false;
            //    }
            //}
        #endregion
    }
}

