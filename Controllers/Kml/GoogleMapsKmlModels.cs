using System.Xml.Serialization;

namespace AlbaClient.Kml
{

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot(Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false, ElementName="kml")]
    public partial class GoogleMapsKml
    {
        public Document Document { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class Document
    {
        public string name { get; set; }

        public string description { get; set; }

        [XmlElement("Style")]
        public Style[] Styles { get; set; }

        [XmlElement("StyleMap")]
        public StyleMap[] StyleMaps { get; set; }

        [XmlElement("Folder")]
        public DocumentFolder[] Folder { get; set; }

        [XmlElementAttribute("Placemark")]
        public Placemark[] Placemark { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class DocumentFolder
    {
        public string name { get; set; }

        [XmlElement("Placemark")]
        public Placemark[] Placemark { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class StyleMap
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlElement("Pair")]
        public Pair[] Pairs { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class Pair
    {
        public string key { get; set; }
        public string styleUrl { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class Style
    {
        [XmlAttribute]
        public string id { get; set; }
        public LineStyle LineStyle { get; set; }
        public PolyStyle PolyStyle { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class LineStyle
    {
        public string color { get; set; }
        public double width { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class PolyStyle
    {
        public string color { get; set; }
        // Boolean 0 or 1
        public int fill { get; set; } 
        // Boolean 0 or 1
        public int outline { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class MultiGeometry
    {
        [XmlElement("Polygon")]
        public PlacemarkPolygon[] Polygon { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class Placemark
    {
        public string styleUrl { get; set; }

        public string name { get; set; }

        [XmlElement("ExtendedData")]
        public ExtendedData ExtendedData { get; set; }

        public string description { get; set; }

        public PlacemarkPoint Point { get; set; }

        [XmlElement("MultiGeometry")]
        public MultiGeometry MultiGeometry { get; set; }

        public PlacemarkPolygon Polygon { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class ExtendedData
    {
        [XmlElement("Data")]
        public Data[] Data { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class Data
    {
        [XmlElement]
        public string value { get; set; }

        [XmlAttribute()]
        public string name { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class PlacemarkPoint
    {
        public string coordinates { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class PlacemarkPolygon
    {
        public OuterBoundaryIs outerBoundaryIs { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class OuterBoundaryIs
    {
        public LinearRing LinearRing { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class LinearRing
    {
        public byte tessellate { get; set; }

        public string coordinates { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class kmlDocumentStyleLineStyle
    {
        public string color { get; set; }

        public byte width { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class kmlDocumentStylePolyStyle
    {
        public string color { get; set; }

        public byte fill { get; set; }

        public byte outline { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class kmlDocumentStyleIconStyle
    {
        public string color { get; set; }

        public decimal scale { get; set; }

        public kmlDocumentStyleIconStyleIcon Icon { get; set; }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.opengis.net/kml/2.2")]
    public partial class kmlDocumentStyleIconStyleIcon
    {
        public string href { get; set; }
    }
}
