using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LogoGen.Settings;

namespace LogoGen.Gui
{
    public class BatchItemSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BatchItemSettings Settings { get; private set; }

        public BatchItemSettingsViewModel(BatchItemSettings settings)
        {
            Settings = settings;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string OutputPath
        {
            get { return Settings.OutputPath; }
            set
            {
                Settings.OutputPath = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get { return Settings.Width; }
            set
            {
                Settings.Width = value;
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get { return Settings.Height; }
            set
            {
                Settings.Height = value;
                OnPropertyChanged();
            }
        }

        public float? Scale
        {
            get { return Settings.Scale; }
            set
            {
                Settings.Scale = value;
                OnPropertyChanged();
            }
        }

        public bool HasScale
        {
            get { return Settings.Scale.HasValue; }
            set
            {
                Settings.Scale = value ? (float?)1.0f : null;
                OnPropertyChanged();
                OnPropertyChanged("Scale");
            }
        }

        public Color? BackgroundColor
        {
            get { return Settings.BackgroundColor.HasValue ? DrawingToMediaColor(Settings.BackgroundColor.Value) : (Color?)null; }
            set
            {
                Settings.BackgroundColor = value.HasValue ? MediaToDrawingColor(value.Value) : (System.Drawing.Color?)null;
                OnPropertyChanged();
            }
        }

        public bool HasBackgroundColor
        {
            get { return Settings.BackgroundColor.HasValue; }
            set
            {
                Settings.BackgroundColor = value ? (System.Drawing.Color?) System.Drawing.Color.White : null;
                OnPropertyChanged();
                OnPropertyChanged("BackgroundColor");
            }
        }

        static Color DrawingToMediaColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        static System.Drawing.Color MediaToDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}