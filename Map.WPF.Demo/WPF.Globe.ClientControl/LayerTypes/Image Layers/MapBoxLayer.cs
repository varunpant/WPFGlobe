
using System;

namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class MapBoxLayer : ImageTileLayer
    {

        private const string TilePathBase = @"https://{0}.tiles.mapbox.com/v3/{1}/{2}/{3}/{4}.png";

        string[] domains = new string[] { "a", "b", "c", "d" };

        int i = 0;
        public string getDomain()
        {
            if (i == 4)
            {
                i = 0;
            }
            return domains[i++];
           
        }

        public enum MapBoxModes
        {
            MapOfDead,
            FourSquare,
            FinancialTimes,
            Washingtonpost,
            Greenpeace,
            Newyork,
            NaturalEarth,
            Streets,
            Streets2,
            Streets3,
            Terrain,
            Satellite,
            Satellite2,
            Satellite3
        }
        /// <summary>
        /// Current Image Mode.
        /// </summary>
        public MapBoxModes MapMode { get; set; }

        public override string GetImageUrl(int column, int row, int zoomLevel)
        {
            string url = "";

            switch (MapMode)
            {
                case MapBoxModes.MapOfDead:
                    url = string.Format(TilePathBase, getDomain(), "jeffmerrick.map-tnw3k3na", zoomLevel, column, row);
                    break;
                case MapBoxModes.Greenpeace:
                    url = string.Format(TilePathBase, getDomain(), "greenpeace-marine.greenpeace-marine-reserve", zoomLevel, column, row);
                    break;
                case MapBoxModes.Newyork:
                    url = string.Format(TilePathBase, getDomain(), "cunycur.nyc1940basemap", zoomLevel, column, row);
                    break;
                case MapBoxModes.Washingtonpost:
                    url = string.Format(TilePathBase, getDomain(), "washingtonpost.map-cbqyo4ce", zoomLevel, column, row);
                    break;
                case MapBoxModes.FourSquare:
                    url = string.Format(TilePathBase, getDomain(), "foursquare.m3elv7vi", zoomLevel, column, row);
                    break;
                case MapBoxModes.FinancialTimes:
                    url = string.Format(TilePathBase, getDomain(), "financialtimes.map-w7l4lfi8", zoomLevel, column, row);
                    break;
                case MapBoxModes.NaturalEarth:
                    url = string.Format(TilePathBase, getDomain(), "mapbox.natural-earth-hypso-bathy", zoomLevel, column, row);
                    break;
                case MapBoxModes.Streets:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-i87786ca", zoomLevel, column, row);
                    break;
                case MapBoxModes.Streets2:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-e3z60292", zoomLevel, column, row);
                    break;
                 case MapBoxModes.Streets3:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-cnkhv76", zoomLevel, column, row);
                    break;
                case MapBoxModes.Terrain:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-i875mjb7", zoomLevel, column, row);
                    break;
                case MapBoxModes.Satellite:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-qfyrx5r8", zoomLevel, column, row);
                    break;
                case MapBoxModes.Satellite2:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-b70jh5xu", zoomLevel, column, row);
                    break;
                case MapBoxModes.Satellite3:
                    url = string.Format(TilePathBase, getDomain(), "examples.map-8ly8i7pv", zoomLevel, column, row);
                    break;

            }

            return url;
        }

        public override string[] MapModes
        {
            get { return System.Enum.GetNames(typeof(MapBoxModes)); }
        }

        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (MapBoxModes)Enum.Parse(typeof(MapBoxModes), NewMode);
        }

        public override string CurrentMapMode
        {
            get
            {
                return MapMode.ToString();
            }

        }
    }
}
