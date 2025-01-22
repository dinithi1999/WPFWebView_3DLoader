using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // Add other properties as needed
    }

    public class ModelLoader
    {
        private string _configFilePath = @"Assets/modelsConfig.xml"; // Path to XML config file

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
    }
}
