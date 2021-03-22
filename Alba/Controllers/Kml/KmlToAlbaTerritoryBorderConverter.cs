using System;
using System.Collections.Generic;
using TerritoryTools.Alba.Controllers.Models;

namespace TerritoryTools.Alba.Controllers.Kml
{
    public class KmlToAlbaTerritoryBorderConverter
    {
        List<AlbaTerritoryBorder> territories;

        public List<AlbaTerritoryBorder> TerritoryListFrom(GoogleMapsKml kml)
        {
            try
            {
                return TerritoriesFrom(kml.Document);
            }
            catch (Exception)
            {
                throw;
            }
        }

        List<AlbaTerritoryBorder> TerritoriesFrom(Document document)
        {
            territories = new List<AlbaTerritoryBorder>();

            if (document?.Folder != null)
                FromFolders(document.Folder);

            if (document?.Placemark != null)
                FromPlacemarks(document.Placemark);

            return territories;
        }

        void FromFolders(DocumentFolder[] folders)
        {
            foreach (var folder in folders)
                if (folder.Placemark != null)
                    FromPlacemarks(folder.Placemark);
        }

        void FromPlacemarks(Placemark[] placemarks)
        {
            foreach (var placemark in placemarks)
                territories.Add(PlacemarkConverterToAlbaTerritoryBorder.From(placemark));
        }
    }
}
