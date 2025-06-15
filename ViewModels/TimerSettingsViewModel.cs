using Caliburn.Micro;
using DBF.DataModel;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DBF.ViewModels
{
    public class TimerSettingViewModel : Screen
    {
        private          Preset         selectedPreset;
        private          TimerSetting   setting;
        private readonly IWindowManager _windowManager;
        //private bool _volumeInitialized = false;
        //private AudioPlayer _audioPlayer;
        public readonly Configuration Configuration;

        public static ObservableCollection<CustomColor> NewColorCollection { get; set; } = new()
        {
            new CustomColor { ColorName = "Rød", Color = (Color)ColorConverter.ConvertFromString("#F2460D")},
            new CustomColor { ColorName = "Blå", Color = (Color)ColorConverter.ConvertFromString("#00b0ff") },
            new CustomColor { ColorName = "range", Color = (Color)ColorConverter.ConvertFromString("#FF9D00")},
            new CustomColor { ColorName = "Grøn", Color = (Color)ColorConverter.ConvertFromString("#81C784")},
        };

        public TimerSettingViewModel(Configuration configuration)
        {
            Configuration = configuration;
            _windowManager              = IoC.Get<IWindowManager>();
            NewSetting.PropertyChanged += onSettingPropertyCahnged;            
        }

        private void onSettingPropertyCahnged(object sender, PropertyChangedEventArgs e)
        {
            if (selectedPreset is not null
            &&  sender         is Preset preset
            &&  !preset.CustomPreset)
            {
                if (!preset.Matches(selectedPreset))
                {
                    SelectedPreset = null;
                    preset.Name    = null;
                }
            }
        }

        public        TimerSetting                      NewSetting         { get; set; } = new();
        public TimerSetting Setting
        {
            get => setting;
            set
            {
                Set(ref setting, value);
                NewSetting.Update(value);

                selectedPreset = FindPreset(NewSetting);
                //    NotifyOfPropertyChange(nameof(SelectedPreset));
                //    NotifyOfPropertyChange(nameof(Setting));
            }
        }

        public        Color                             BackgroundColor    { get; set; }
        
        public Preset SelectedPreset
        {
            get => selectedPreset;
            set
            {
                selectedPreset = value;

                if (value != null)
                    NewSetting.Update(value);
            }
        }
        

        public async void Cancel()
        {
            await TryCloseAsync();
        }

        public async void AcceptSetting()
        {
            Setting.Update(NewSetting);
            await TryCloseAsync();
            Configuration.SaveSettings();
        }

        public async void AddPreset()
        {
            var dialog = IoC.Get<PresetNameViewModel>();
            await _windowManager.ShowDialogAsync(dialog);

            if (!string.IsNullOrEmpty(dialog.PresetName))
            {
                NewSetting.Name = dialog.PresetName;
                var preset = new Preset(NewSetting) ;
                Configuration.Presets.Add(preset);

                Configuration.SavePresets();
                SelectedPreset = preset;
            }
        }

        public void SavePreset()
        {
            SelectedPreset.Update(NewSetting);
            Configuration.SavePresets();
        }

        public void DeletePreset()
        {
            if (SelectedPreset.CustomPreset)
            {
                Configuration.Presets.Remove(selectedPreset);
                Configuration.SavePresets();
                NewSetting.Name = null;
                SelectedPreset  = FindPreset(NewSetting);
            }
        }

        public void VolumeChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            double newValue = e.NewValue;
            AudioPlayer.Play(NewSetting.Sound, (int)newValue);
      
        }

        public void SoundChanged()
        {            
            AudioPlayer.Play(NewSetting.Sound, (int) NewSetting.Volume);
            
        }

        private Preset FindPreset(Preset preset) => Configuration.Presets.FirstOrDefault(p => p.Matches(preset));
    }
}