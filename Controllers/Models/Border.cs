﻿using System.Collections.Generic;

namespace AlbaClient.Models
{
    /// <summary>
    /// Territory Border.
    /// </summary>
    public class Border
    {
        public Border()
        {
            Vertices = new List<Vertex>();
        }

        /// <summary>
        /// Coordinates, Longitude and Latitude.
        /// </summary>
        public List<Vertex> Vertices { get; set; }
    }
}