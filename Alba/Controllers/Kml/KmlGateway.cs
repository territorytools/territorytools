using System.IO;
using System.Xml.Serialization;

namespace Alba.Controllers.Kml
{
    public class KmlGateway
    {
        public GoogleMapsKml Load(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new FileNotFoundException("No file was selected.");

            var serializer = new XmlSerializer(typeof(GoogleMapsKml));
            var stream = File.OpenRead(fileName);
            var kml = serializer.Deserialize(stream) as GoogleMapsKml;
            return kml;
        }

        public void Save(string fileName, GoogleMapsKml kml)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var serializer = new XmlSerializer(typeof(GoogleMapsKml));
            var stream = File.Create(fileName);
            serializer.Serialize(stream, kml);
            stream.Flush();
            stream.Close();
        }
    }
}
