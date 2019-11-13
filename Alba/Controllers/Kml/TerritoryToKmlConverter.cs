﻿using System;
using System.Collections.Generic;
using AlbaClient.Models;

namespace AlbaClient.Kml
{
    public class TerritoryToKmlConverter
    {
        public GoogleMapsKml KmlFrom(IEnumerable<Territory> territories)
        {
            try
            {
                return new GoogleMapsKml()
                {
                    Document = DocumentFrom(territories)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        Document DocumentFrom(IEnumerable<Territory> territories)
        {
            return new Document()
            {
                Folder = FoldersFrom(territories),
                Styles = StylesFrom(territories),
                StyleMaps = StyleMapsFrom(territories)
            };
        }

        DocumentFolder[] FoldersFrom(IEnumerable<Territory> territories)
        {
            return new DocumentFolder[]
            {
                new DocumentFolder()
                {
                    Placemark = PlacemarksFrom(territories),
                }
            };
        }

        Placemark[] PlacemarksFrom(IEnumerable<Territory> territories)
        {
            var placemarks = new List<Placemark>();
            foreach (var territory in territories)
                placemarks.Add(new PlacemarkConverter().PlacemarkFrom(territory));

            return placemarks.ToArray();
        }

        Style[] StylesFrom(IEnumerable<Territory> territories)
        {
            var styles = new List<Style>();

            foreach (var t in territories)
            {
                string color = PlacemarkConverter.ColorString(t.FillColor);
                string style = $"t-fill-color-{color}";
                if (!styles.Exists(s => s.id.StartsWith(style)))
                {
                    styles.Add(
                        new Style
                        {
                            id = $"{style}-normal",
                            LineStyle = new LineStyle
                            {
                                color = "ff000000",
                                width = 1.0
                            },
                            PolyStyle = new PolyStyle
                            {
                                color = color,
                                fill = 1,
                                outline = 1
                            }
                        });
                    styles.Add(
                       new Style
                       {
                           id = $"{style}-highlight",
                           LineStyle = new LineStyle
                           {
                               color = "ff000000",
                               width = 1.0
                           },
                           PolyStyle = new PolyStyle
                           {
                               color = color,
                               fill = 1,
                               outline = 1
                           }
                       });
                }
            }

            return styles.ToArray();
        }

        StyleMap[] StyleMapsFrom(IEnumerable<Territory> territories)
        {
            var maps = new List<StyleMap>();

            foreach (var t in territories)
            {
                string style = $"t-fill-color-{PlacemarkConverter.ColorString(t.FillColor)}";
                if (!maps.Exists(s => s.id.Equals(style)))
                {
                    maps.Add(
                        new StyleMap
                        {
                            id = style,
                            Pairs = new Pair[]
                            {
                                new Pair
                                {
                                    key = "normal",
                                    styleUrl = $"#{style}-normal"
                                },
                                new Pair
                                {
                                    key = "highlight",
                                    styleUrl = $"#{style}-highlight"
                                },
                            }
                        });
                }
            }

            return maps.ToArray();
        }
    }
}
