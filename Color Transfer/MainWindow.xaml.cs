using Color_Transfer.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Color_Transfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel mainViewModel = new();

        private const string MATERIAL_BASE_COLOR = "_Base_Col";
        private const string MATERIAL_RED_COLOR = "_Red_Col";
        private const string MATERIAL_GREEN_COLOR = "_Green_Col";
        private const string MATERIAL_BLUE_COLOR = "_Blue_Col";

        private const string SWATCH_BASE_COLOR = "ColorBase";
        private const string SWATCH_RED_COLOR = "ColorR";
        private const string SWATCH_GREEN_COLOR = "ColorG";
        private const string SWATCH_BLUE_COLOR = "ColorB";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void DropMaterial(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                mainViewModel.MaterialPath = files[0];
            }
        }

        private void DropSwatch(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                mainViewModel.SwatchPath = files[0];
            }
        }

        private void BrowseToMaterial(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Open Material File";
            //openFileDialog.DefaultExt = ".json";
            //openFileDialog.Filter = "json Documents (.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                mainViewModel.MaterialPath = openFileDialog.FileName;
            }
        }

        private void BrowseToSwatch(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Open Swatch File";
            //openFileDialog.DefaultExt = ".json";
            //openFileDialog.Filter = "json Documents (.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                mainViewModel.SwatchPath = openFileDialog.FileName;
            }
        }

        private void TransferColors(object sender, RoutedEventArgs e)
        {
            string materialText = File.ReadAllText(mainViewModel.MaterialPath);
            string swatchText = File.ReadAllText( mainViewModel.SwatchPath);

            Match materialMatchBase = GetMaterialMatch(materialText, MATERIAL_BASE_COLOR);
            Match materialMatchRed = GetMaterialMatch(materialText, MATERIAL_RED_COLOR);
            Match materialMatchGreen = GetMaterialMatch(materialText, MATERIAL_GREEN_COLOR);
            Match materialMatchBlue = GetMaterialMatch(materialText, MATERIAL_BLUE_COLOR);

            Match swatchMatchBase = GetSwatchMatch(swatchText, SWATCH_BASE_COLOR);
            Match swatchMatchRed = GetSwatchMatch(swatchText, SWATCH_RED_COLOR);
            Match swatchMatchGreen = GetSwatchMatch(swatchText, SWATCH_GREEN_COLOR);
            Match swatchMatchBlue = GetSwatchMatch(swatchText, SWATCH_BLUE_COLOR);

            string result;
            result = ReplaceColors(swatchText, materialMatchBlue, swatchMatchBlue);
            result = ReplaceColors(result, materialMatchGreen, swatchMatchGreen);
            result = ReplaceColors(result, materialMatchRed, swatchMatchRed);
            result = ReplaceColors(result, materialMatchBase, swatchMatchBase);

            File.WriteAllText(mainViewModel.SwatchPath, result);
        }

        private static Match GetMaterialMatch(string input, string colorName)
        {
            string materialPattern = $"\t+string first = \"{colorName}\"\r\n\t+ColorRGBA second\r\n\t+float r = ([01]\\.?\\d*)\r\n\t+float g = ([01]\\.?\\d*)\r\n\t+float b = ([01]\\.?\\d*)\r\n\t+float a = ([01]\\.?\\d*)\r\n";
            Match materialMatch = Regex.Match(input, materialPattern, RegexOptions.Multiline);

            return materialMatch;
        }

        private static Match GetSwatchMatch(string input, string colorName)
        {
            string swatchPattern = $"^\\s+\"{colorName}\": {{\r\n\\s+\"r\": (0\\.0),\r\n\\s+\"g\": (0\\.0),\r\n\\s+\"b\": (0\\.0),\r\n\\s+\"a\": (1\\.0)\r\n";
            Match swatchMatch = Regex.Match(input, swatchPattern, RegexOptions.Multiline);

            return swatchMatch;
        }

        private static string ReplaceColors(string input, Match materialMatch, Match swatchMatch)
        {
            StringBuilder replaced = new(input);
            if (materialMatch.Success && swatchMatch.Success)
            {
                string materialRed = materialMatch.Groups[1].Value;
                string materialGreen = materialMatch.Groups[2].Value;
                string materialBlue = materialMatch.Groups[3].Value;
                string materialAlpha = materialMatch.Groups[4].Value;
                Capture redCapture = swatchMatch.Groups[1].Captures[0];
                Capture greenCapture = swatchMatch.Groups[2].Captures[0];
                Capture blueCapture = swatchMatch.Groups[3].Captures[0];
                Capture alphaCapture = swatchMatch.Groups[4].Captures[0];

                replaced.Remove(alphaCapture.Index, alphaCapture.Length);
                replaced.Insert(alphaCapture.Index, materialAlpha);
                replaced.Remove(blueCapture.Index, blueCapture.Length);
                replaced.Insert(blueCapture.Index, materialBlue);
                replaced.Remove(greenCapture.Index, greenCapture.Length);
                replaced.Insert(greenCapture.Index, materialGreen);
                replaced.Remove(redCapture.Index, redCapture.Length);
                replaced.Insert(redCapture.Index, materialRed);
            }
            return replaced.ToString();
        }

        private void HighlightDrop(object sender, DragEventArgs e)
        {
            if (sender is Label label)
            {
                label.BorderBrush = Brushes.Teal;
            }
        }

        private void UnhighlightDrop(object sender, DragEventArgs e)
        {
            if (sender is Label label)
            {
                label.BorderBrush = Brushes.LightGray;
            }
        }
    }
}
