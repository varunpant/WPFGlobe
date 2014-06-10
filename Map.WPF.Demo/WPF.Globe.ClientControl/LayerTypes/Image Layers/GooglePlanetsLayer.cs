
using System;
using System.Text;
namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class GooglePlanetsLayer : ImageTileLayer
    {
        public enum GpMapModes
        {
            GoogleMoon,
            GoogleMoonClemBw,
            GoogleMoonTerrain,
            GoogleMarsInfraRed,
            GoogleMarsElevation,
            GoogleMarsVisible
        }

        public GpMapModes MapMode { get; set; }

        private const string TilePathGoogleMarsE = @"http://mw1.google.com/mw-planetary/mars/elevation/t{0}.jpg";
        private const string TilePathGoogleMarsIR = @"http://mw1.google.com/mw-planetary/mars/infrared/t{0}.jpg";
        private const string TilePathGoogleMarsV = @"http://mw1.google.com/mw-planetary/mars/visible/t{0}.jpg";
        private const string TilePathGoogleMoon = @"http://mw1.google.com/mw-planetary/moon/t{0}.jpg";
        private const string TilePathGoogleMoonC = "http://mw1.google.com/mw-planetary/lunar/lunarmaps_v1/clem_bw/{0}/{1}/{2}.jpg";
        private const string TilePathGoogleMoonT = "http://mw1.google.com/mw-planetary/lunar/lunarmaps_v1/terrain/{0}/{1}/{2}.jpg";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public override string GetImageUrl(int column, int row, int zoomLevel)
        {
          
            var code = GetVENumber(column, row, zoomLevel);

            QuadKeyToTileXY(code, out row, out column, out zoomLevel);

            return GetTile(zoomLevel, row, column).AbsoluteUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <returns></returns>
        public Uri GetTile(int zoom, int tilePositionX, int tilePositionY)
        {
            string url = string.Empty;

            switch (MapMode)
            {
                case GpMapModes.GoogleMoon:
                    url = QuadKeyNumberToAlphaUrl(TilePathGoogleMoon, tilePositionX, tilePositionY, zoom);
                    break;
                case GpMapModes.GoogleMoonClemBw:
                    url = XYZUrl(TilePathGoogleMoonC, tilePositionX, tilePositionY, zoom);
                    break;
                case GpMapModes.GoogleMoonTerrain:
                    url = XYZUrl(TilePathGoogleMoonT, tilePositionX, tilePositionY, zoom);
                    break;
                case GpMapModes.GoogleMarsInfraRed:
                    url = QuadKeyNumberToAlphaUrl(TilePathGoogleMarsIR, tilePositionX, tilePositionY, zoom);
                    break;
                case GpMapModes.GoogleMarsElevation:
                    url = QuadKeyNumberToAlphaUrl(TilePathGoogleMarsE, tilePositionX, tilePositionY, zoom);
                    break;
                case GpMapModes.GoogleMarsVisible:
                    url = QuadKeyNumberToAlphaUrl(TilePathGoogleMarsV, tilePositionX, tilePositionY, zoom);
                    break;
            }

            return new Uri(url);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private static string XYZUrl(string url, int tilePositionX, int tilePositionY, int zoom)
        {
            tilePositionY = (Convert.ToInt32(Math.Pow(2.0, zoom)) - tilePositionY) - 1;
            url = string.Format(url, zoom, tilePositionX, tilePositionY);

            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private static string QuadKeyNumberToAlphaUrl(string url, int tilePositionX, int tilePositionY, int zoom)
        {
            string quadKey = TileXYToQuadKey(tilePositionX, tilePositionY, zoom);
            string str3 = quadKey.Substring(quadKey.Length - 1, 1);
            string alpha = QuadKeyNumberToAlpha(quadKey);

            return string.Format(url, alpha, str3);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base4"></param>
        /// <returns></returns>
        protected static string QuadKeyNumberToAlpha(string base4)
        {
            return base4.Replace('0', 'q').Replace('1', 'r').Replace('2', 't').Replace('3', 's');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="levelOfDetail"></param>
        /// <returns></returns>
        private static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            var quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        /// <summary>
        /// Return Map Modes.
        /// </summary>
        public override string[] MapModes
        {
            get
            {
                return System.Enum.GetNames(typeof(GpMapModes));
            }
        }

        /// <summary>
        /// Switches Map Mode.
        /// </summary>
        /// <param name="NewMode"></param>
        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (GpMapModes)Enum.Parse(typeof(GpMapModes), NewMode);
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
