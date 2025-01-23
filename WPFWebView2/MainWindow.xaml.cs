using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace WPFWebView2
{
    public partial class MainWindow : Window
    {
        // List of models loaded from XML config file
        private List<Model> models;
        private Model selectedModel;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        /// <summary>
        /// Asynchronously initialize WebView2 and load local HTML file.
        /// Load models from XML config file and update index.html.
        /// </summary>
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

        /// <summary>
        /// Event handler for when the selected item in the object selector combobox changes.
        /// Updates the selected model based on the selected item.
        /// </summary>
        private void ObjectSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedId = objectSelector.SelectedItem as string;
            selectedModel = models.Find(m => m.Id == selectedId);
        }

        /// <summary>
        /// Event handler for when the text in the translation text boxes changes.
        /// Updates the translation of the selected model and applies the changes.
        /// </summary>
        private async void TranslationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedModel != null)
            {
                selectedModel.Translation = $"{translationXTextBox.Text},{translationYTextBox.Text},{translationZTextBox.Text}";
                await UpdateModelTransformAsync();
            }
        }

        /// <summary>
        /// Event handler for when the text in the rotation text boxes changes.
        /// Updates the rotation of the selected model and applies the changes.
        /// </summary>
        private async void RotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedModel != null)
            {
                selectedModel.Rotation = $"{rotationXTextBox.Text},{rotationYTextBox.Text},{rotationZTextBox.Text}";
                await UpdateModelTransformAsync();
            }
        }

        /// <summary>
        /// Updates the transform (position and rotation) of the selected model in the Babylon.js scene.
        /// </summary>
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

        /// <summary>
        /// Event handler for when the selected color in the color picker changes.
        /// Updates the color of the selected model and applies the changes.
        /// </summary>
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

        /// <summary>
        /// Applies the changes to the selected model by updating its transform and color.
        /// </summary>
        private async void ApplyChangesAsync()
        {
            if (selectedModel != null)
            {
                await UpdateModelTransformAsync();
            }
        }

        /// <summary>
        /// Ensures that the given vector string is valid and returns it.
        /// If the vector string is invalid, returns the default value.
        /// </summary>
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


