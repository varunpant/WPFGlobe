using System;

namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class GoogleEarthMapLayer : ImageTileLayer
    {
        public enum GoogleMapModes
        {
            Street,
            Satellite,
            SatelliteHybrid,
            Physical,
            PhysicalHybrid,
            StreetOverlay,
            StreetWaterOverlay
        }    


        private const string TilePathBase = @"http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}";

        private const string charStreet = "m";
        private const string charSatellite = "s";
        private const string charSatelliteHybrid = "y";
        private const string charPhysical = "t";
        private const string charPhysicalHybrid = "p";
        private const string charStreetOverlay = "h";
        private const string charStreetWaterOverlay = "r";

        private int server_rr = 0;

        public override string GetImageUrl(int column, int row, int zoomLevel)
        {        
            var code = GetVENumber(column, row, zoomLevel);

            QuadKeyToTileXY(code, out row, out column, out zoomLevel);
         
            return GetTile(zoomLevel, row, column).AbsoluteUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileLevel"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <returns></returns>
        public Uri GetTile(int tileLevel, int tilePositionX, int tilePositionY)
        {
           
                int zoom = tileLevel;             

                string url = string.Empty;
                server_rr = (server_rr + 1) % 4;

                switch (MapMode)
                {
                    case GoogleMapModes.Street:
                        url = XYZUrl(TilePathBase, server_rr, charStreet, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.Satellite:
                        url = XYZUrl(TilePathBase, server_rr, charSatellite, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.SatelliteHybrid:
                        url = XYZUrl(TilePathBase, server_rr, charSatelliteHybrid, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.Physical:
                        url = XYZUrl(TilePathBase, server_rr, charPhysical, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.PhysicalHybrid:
                        url = XYZUrl(TilePathBase, server_rr, charPhysicalHybrid, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.StreetOverlay:
                        url = XYZUrl(TilePathBase, server_rr, charStreetOverlay, zoom, tilePositionX, tilePositionY);
                        break;
                    case GoogleMapModes.StreetWaterOverlay:
                        url = XYZUrl(TilePathBase, server_rr, charStreetWaterOverlay, zoom, tilePositionX, tilePositionY);
                        break;
                }

                return new Uri(url);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="server"></param>
        /// <param name="mapmode"></param>
        /// <param name="zoom"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <returns></returns>
        private static string XYZUrl(string url, int server, string mapmode, int zoom, int tilePositionX, int tilePositionY)
        {
            url = string.Format(url, server, mapmode, zoom, tilePositionX, tilePositionY);

            return url;
        }

        /// <summary>
        /// Current Image Mode.
        /// </summary>
        public GoogleMapModes MapMode { get; set; }

        /// <summary>
        /// Return Map Modes.
        /// </summary>
        public override string[] MapModes
        {
            get
            {
                return System.Enum.GetNames(typeof(GoogleMapModes));
            }
        }
        
        /// <summary>
        /// Switches Map Mode.
        /// </summary>
        /// <param name="NewMode"></param>
        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (GoogleMapModes)Enum.Parse(typeof(GoogleMapModes), NewMode);
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
