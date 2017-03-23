using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LogoGen.Settings;
using Microsoft.Win32;
using Newtonsoft.Json;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace LogoGen.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<BatchItemSettingsViewModel> _items;

        public MainWindow()
        {
            InitializeComponent();
            _items = new ObservableCollection<BatchItemSettingsViewModel>();

            ItemSettings.ItemsSource = _items;
        }

        void LoadSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var path = GetOpenFilePath("json");
            if (path == null) return;

            ReadSettings(path);
        }

        void SaveSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var path = GetSaveFilePath("json");
            if (path == null) return;

            WriteSettings(path);
        }

        void BrowseSvg_OnClick(object sender, RoutedEventArgs e)
        {
            var path = GetOpenFilePath("svg");
            if (path == null) return;

            SvgPath.Text = path;
        }

        void AddItem_OnClick(object sender, RoutedEventArgs e)
        {
            _items.Add(new BatchItemSettingsViewModel(new BatchItemSettings(100, 100, null)));
        }

        async void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            Errors.Content = "";
            var settings = GetBatchSettings();
            var results = await Task.Run(() => new LogoGenerator().GenerateBatch(settings));
            Errors.Content = $"{results.Count(x => x.Succeeded)} successes, {results.Count(x => !x.Succeeded)} failures.";
        }

        void BrowsePng_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;

            var path = GetOpenFilePath("png");
            if (path == null) return;

            var settings = (BatchItemSettingsViewModel)button.DataContext;
            settings.OutputPath = path;

            if (!File.Exists(path)) return;

            var image = Image.FromFile(path);
            settings.Width = image.Width;
            settings.Height = image.Height;
        }

        void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var settings = (BatchItemSettingsViewModel)button.DataContext;
            _items.Remove(settings);
        }

        void ReadSettings(string path)
        {
            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<BatchSettings>(json);

            DefaultBackgroundColor.SelectedColor = System.Windows.Media.Color.FromArgb(
                settings.BackgroundColor.A,
                settings.BackgroundColor.R,
                settings.BackgroundColor.G,
                settings.BackgroundColor.B);

            DefaultScale.Value = settings.Scale;
            SvgPath.Text = settings.SvgPath;

            _items = new ObservableCollection<BatchItemSettingsViewModel>(
                settings.ItemSettings.Select(s => new BatchItemSettingsViewModel(s)));
            ItemSettings.ItemsSource = _items;
        }

        void WriteSettings(string path)
        {
            var json = JsonConvert.SerializeObject(GetBatchSettings(), Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            File.WriteAllText(path, json);
        }

        BatchSettings GetBatchSettings()
        {
            var mediaColor = DefaultBackgroundColor.SelectedColor ?? Colors.Transparent;
            var color = Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
            var batchSettings = new BatchSettings(SvgPath.Text, DefaultScale.Value ?? 1.0f, color, true,
                _items.Select(i => i.Settings).ToArray());

            return batchSettings;
        }

        string GetOpenFilePath(string fileType)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = $".{fileType}",
                Filter = $"{fileType.ToUpper()} Files (*.{fileType})|*.{fileType}",
                CheckFileExists = false
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        string GetSaveFilePath(string fileType)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = $".{fileType}",
                Filter = $"{fileType.ToUpper()} Files (*.{fileType})|*.{fileType}",
                CheckFileExists = false
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}
