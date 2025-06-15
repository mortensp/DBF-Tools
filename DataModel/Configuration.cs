using Caliburn.Micro;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace DBF.DataModel
{
    public class Configuration : PropertyChangedBase
    {
        private static readonly string[] audioExtensions   = new[] { ".wav", ".mp3" };
        private int                      visibleTimerCount = 0;

        public ObservableCollection <CustomColor> BackgroundColors;

        public        BindableCollection<TimerSetting> TimerSettings { get; set; } = new();
        public        BindableCollection<Preset>       Presets       { get; set; } = new()
        {
                new Preset("Par - 7 runder af 4 spil", false, false,  7, 4, 4, 0,27, 0, 1, 12),
                new Preset("Par - 9 runder af 3 spil", false, false,  9, 3,  5,0, 20, 0, 1, 12),
                new Preset("Par - 11 runder af 2 spil",false, false, 11, 2,  6,0,13, 0, 1, 12),
                new Preset("Hold kamp af 32 spil",     false, true,   2, 16, 1,1,28, 0, 0,15)
        };

        private string configDir, presetsPath, timerSettingPath;
                string jsonData,  jsonPresets, jsonTimers;

        public  bool                             TimersCanClose  { get; set; }
        public bool TimersCanBeAdded{ get; set; }

        public Configuration()
        {
            BackgroundColors = new ObservableCollection<CustomColor>();

            BackgroundColors.Add(new CustomColor() { Color = (Color)ColorConverter.ConvertFromString("#F2460D"), ColorName = "Rød (dbf)" });
            BackgroundColors.Add(new CustomColor() { Color = (Color)ColorConverter.ConvertFromString("#00b0ff"), ColorName = "Blå (dbf)" });
            BackgroundColors.Add(new CustomColor() { Color = (Color)ColorConverter.ConvertFromString("#FF9D00"), ColorName = "Orange (dbf)" });
            BackgroundColors.Add(new CustomColor() { Color = (Color)ColorConverter.ConvertFromString("#81C784"), ColorName = "Grøn (dbf)" });

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // Roaming folder
            configDir          = Path.Combine(appDataPath, "DBFTools");
            presetsPath        = Path.Combine(configDir  , "presets.json");
            timerSettingPath   = Path.Combine(configDir  , "timerSetting.json");

            // Make sure that directory and files exists
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            if (!File.Exists(presetsPath))
            {
                string json = JsonSerializer.Serialize( Presets.Where(p => p.CustomPreset == true).ToObservableCollection<Preset>()
                                                      , new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(presetsPath, json);
            }

            if (!File.Exists(timerSettingPath))
            {
                string json = JsonSerializer.Serialize(TimerSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(timerSettingPath, json);
            }

            TimerSettings.CollectionChanged += TimerSettings_CollectionChanged;

            Load();
        }

        private void TimerSettings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
               foreach (TimerSetting newItem in e.NewItems)
                    newItem.PropertyChanged += TimerSetting_PropertyChanged;

            if (e.OldItems != null)
                foreach (TimerSetting oldItem in e.OldItems)
                    oldItem.PropertyChanged -= TimerSetting_PropertyChanged;
        }

        private void TimerSetting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as TimerSetting;

            if (item != null && e.PropertyName == nameof(TimerSetting.Visibility))
            {
                if (item.Visibility == Visibility.Visible)
                    visibleTimerCount++;
                else
                    visibleTimerCount--;

                TimersCanClose = visibleTimerCount > 1;
                TimersCanBeAdded = visibleTimerCount < 4;
            }
        }

        internal void SavePresets()
        {
            jsonPresets = JsonSerializer.Serialize<ObservableCollection<Preset>>(Presets.Where(p => p.CustomPreset == true).ToObservableCollection<Preset>());

            File.WriteAllText(presetsPath, jsonPresets);
        }

        internal void SaveSettings()
        {
            jsonTimers = JsonSerializer.Serialize<ObservableCollection<TimerSetting>>(TimerSettings);

            File.WriteAllText(timerSettingPath, jsonTimers);

            visibleTimerCount = TimerSettings.Count(ts => ts.Visibility == Visibility.Visible);
            //TimersCanClose      = visibleTimerCount >  1;
            //TimersCanBeAdded = visibleTimerCount < 4;
        }

        internal void Save()
        {
            SavePresets();
            SaveSettings();
        }

        internal void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var      names    = assembly.GetManifestResourceNames();

            foreach (var name in names.Where(n => n.StartsWith("DBF.AudioFiles.")))
                //audioExtensions.Any(ext => n.EndsWith(ext, StringComparison.OrdinalIgnoreCase))))
                Console.WriteLine(name);

            jsonData          = File.ReadAllText(presetsPath);
            var loadedPresets = JsonSerializer.Deserialize<ObservableCollection<Preset>>(jsonData);

            foreach (var preset in loadedPresets)
                if (Presets.FirstOrDefault(p => p.Name == preset.Name) == null)
                    Presets.Add(preset);

            jsonData         = File.ReadAllText(timerSettingPath);
            var loadedTimers = JsonSerializer.Deserialize<ObservableCollection<TimerSetting>>(jsonData);
            int i;

            for (i = 0; i <  4; i++)
            {
                TimerSetting setting;

                if (loadedTimers is null || loadedTimers.Count == 0)
                {
                    setting = new TimerSetting();
                    setting.Update(Presets[i]);
                    setting.Name  = null;
                    setting.Color = BackgroundColors[i].Color;
                    setting.Group = ((char)('A' + i)).ToString(); // Set group to A, B, C or D
                }
                else
                    if (loadedTimers.Count >  i)
                        setting = loadedTimers[i];
                    else
                    {
                        setting = new();
                        setting.Update(Presets[i]);
                        setting.Visibility = System.Windows.Visibility.Collapsed;
                    }

                if (string.IsNullOrEmpty(setting.Sound))
                    setting.Sound = AudioPlayer.Sounds[i];

                TimerSettings.Add(setting);

                if (setting.Visibility == Visibility.Visible)
                    visibleTimerCount++;
            }

            for (; i <  4; i++)
            {
                var setting = new TimerSetting();
                setting.Update(Presets[i]);
                setting.Name       = null;
                setting.Color      = BackgroundColors[i].Color;
                setting.Group      = ((char)('A' + i)).ToString(); // Set group to A, B, C or D
                setting.Visibility = Visibility.Collapsed;
                setting.Sound      = AudioPlayer.Sounds[i];

                if (setting.Visibility == Visibility.Visible)
                    visibleTimerCount++;
            }

            TimersCanClose   = visibleTimerCount >  1;
            TimersCanBeAdded = visibleTimerCount < 4;
        }
    }
}

