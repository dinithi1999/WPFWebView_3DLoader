using System.IO;
using System.Xml.Linq;

namespace WPFWebView2
{
    public class Model
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public string FilePath { get; set; }
        public string Color { get; set; }
        public string Translation { get; set; }
        public string Rotation { get; set; }
        public string OffsetTranslation { get; set; }
        public string OffsetRotation { get; set; }
    }

    public class ModelLoader
    {
        private string _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot/Assets/modelsConfig.xml"); // Path to XML config file
        private string _indexHtmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\index.html"); // Path to index.html file

        /// <summary>
        ///  Load the models data from the XML config file
        /// </summary>
        /// <returns></returns>
        public List<Model> LoadModels()
        {
            // Load the XML document
            XElement xmlDoc = XElement.Load(_configFilePath);

            // Parse the XML into a list of Model objects
            var models = xmlDoc.Descendants("Model")
                .Where(m => (string)m.Attribute("Show") == "true") // Optional: Only load models with Show="true"
                .Select(m => new Model
                {
                    Id = (string)m.Attribute("Id"),
                    Group = (string)m.Attribute("Group"),
                    FilePath = (string)m.Attribute("FilePath"),
                    Color = (string)m.Attribute("Color"),
                    Translation = (string)m.Attribute("Translation"),
                    Rotation = (string)m.Attribute("Rotation"),
                    OffsetTranslation = (string)m.Attribute("OffsetTranslation"),
                    OffsetRotation = (string)m.Attribute("OffsetRotation")
                }).ToList();

            return models;
        }

        /// <summary>
        /// Update the index.html file with the models data from the XML config file
        /// </summary>
        public void UpdateIndexHtml()
        {
            var models = LoadModels();
            var modelsData = string.Join(",\n", models.Select(m => $@"
                {{
                    Id: '{m.Id}',
                    FilePath: '{m.FilePath}',
                    Translation: '{m.Translation}',
                    Rotation: '{m.Rotation}',
                    OffsetTranslation: '{m.OffsetTranslation}',
                    OffsetRotation: '{m.OffsetRotation}'
                }}"));

            var indexHtmlContent = File.ReadAllText(_indexHtmlFilePath);
            indexHtmlContent = indexHtmlContent.Replace("{{modelsData}}", modelsData);
            File.WriteAllText(_indexHtmlFilePath, indexHtmlContent);
        }
    }
}
