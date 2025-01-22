using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.IO.Path;

namespace WPFWebView2
{
    public partial class MainWindow : Window
    {
        private List<Model> models;
        private Model selectedModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            // Load models and update index.html
            var modelLoader = new ModelLoader();
            models = modelLoader.LoadModels();
            modelLoader.UpdateIndexHtml();

            // Populate object selector combobox
            foreach (var model in models)
            {
                objectSelector.Items.Add(model.Id);
            }

            // Initialize WebView2
            var options = new CoreWebView2EnvironmentOptions("--disable-web-security");
            var environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView.EnsureCoreWebView2Async(environment);

            // Load local HTML file
            string filePath = $"file://{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/index.html")}";
            webView.Source = new Uri(filePath);
        }

        private void ObjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedId = objectSelector.SelectedItem as string;
            selectedModel = models.Find(m => m.Id == selectedId);

            if (selectedModel != null)
            {
                // Parse translation and rotation values
                var translation = ParseVector3(selectedModel.Translation);
                var rotation = ParseVector3(selectedModel.Rotation);

                // Update UI with model data
                translationXTextBox.Text = translation[0].ToString("F2");
                translationYTextBox.Text = translation[1].ToString("F2");
                translationZTextBox.Text = translation[2].ToString("F2");

                rotationXTextBox.Text = rotation[0].ToString("F2");
                rotationYTextBox.Text = rotation[1].ToString("F2");
                rotationZTextBox.Text = rotation[2].ToString("F2");

                colorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(selectedModel.Color);
            }
        }

        private async void TranslationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedModel != null)
            {
                selectedModel.Translation = $"{translationXTextBox.Text},{translationYTextBox.Text},{translationZTextBox.Text}";
                await UpdateModelTransformAsync();
            }
        }

        private async void RotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedModel != null)
            {
                selectedModel.Rotation = $"{rotationXTextBox.Text},{rotationYTextBox.Text},{rotationZTextBox.Text}";
                await UpdateModelTransformAsync();
            }
        }

        private async Task UpdateModelTransformAsync()
        {
            if (selectedModel != null)
            {
                // Ensure valid default values
                string translation = EnsureValidVector3(selectedModel.Translation, "0,0,0");
                string rotation = EnsureValidVector3(selectedModel.Rotation, "0,0,0");

                var script = $@"
                    applyChangesToModel('{selectedModel.Id}', [{translation.Replace(",", ", ")}], [{rotation.Replace(",", ", ")}], '{selectedModel.Color}');
                ";
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (selectedModel != null && e.NewValue.HasValue)
            {
                var color = e.NewValue.Value;
                var hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                selectedModel.Color = hexColor;
                ApplyChangesAsync();
            }
        }

        private async void ApplyChangesAsync()
        {
            if (selectedModel != null)
            {
                await UpdateModelTransformAsync();
            }
        }

        private async void ApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedModel != null)
            {
                await UpdateModelTransformAsync();
            }
        }

        private float[] ParseVector3(string vector)
        {
            try
            {
                return Array.ConvertAll(vector.Split(','), float.Parse);
            }
            catch
            {
                return new float[] { 0f, 0f, 0f }; // Default to (0,0,0) on parsing error
            }
        }

        private string EnsureValidVector3(string vector, string defaultValue)
        {
            try
            {
                var components = vector.Split(',');
                if (components.Length == 3)
                {
                    float.Parse(components[0]);
                    float.Parse(components[1]);
                    float.Parse(components[2]);
                    return vector;
                }
            }
            catch
            {
                // Ignore parsing errors and return default value
            }
            return defaultValue;
        }
    }
}

