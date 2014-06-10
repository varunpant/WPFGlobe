
using System;
namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class YahooMapLayer : ImageTileLayer
    {
        /// <summary>
        /// 
        /// </summary>
        public enum YhooMapModes
        {
            YahooHybrid,
            YahooAerial,
            YahooStreet
        }


        private const string TilePathAerial = @"http://us.maps3.yimg.com/aerial.maps.yimg.com/tile?v=1.7&t=a&x={x}&y={y}&z={z}";
        private const string TilePathHybrid = @"http://us.maps3.yimg.com/aerial.maps.yimg.com/png?v=2.2&t=h&x={x}&y={y}&z={z}";
        private const string TilePathStreet = @"http://us.maps2.yimg.com/us.png.maps.yimg.com/png?v=3.52&t=m&x={x}&y={y}&z={z}";


        public YhooMapModes MapMode { get; set; }

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
        /// <param name="tileLevel"></param>
        /// <param name="tilePositionX"></param>
        /// <param name="tilePositionY"></param>
        /// <returns></returns>
        public Uri GetTile(int tileLevel, int tilePositionX, int tilePositionY)
        {

            int zoom = tileLevel;

            int zoomLevel = 18 - zoom;
            double num4 = Math.Pow(2.0, zoom) / 2.0;
            double y;
            if (tilePositionY < num4)
            {
                y = (num4 - tilePositionY) - 1.0;
            }
            else
            {
                y = ((tilePositionY + 1) - num4) * -1.0;
            }

            string url = string.Empty;

            switch (MapMode)
            {
                case YhooMapModes.YahooHybrid:
                    url = TilePathHybrid;
                    break;
                case YhooMapModes.YahooAerial:
                    url = TilePathAerial;
                    break;
                case YhooMapModes.YahooStreet:
                    url = TilePathStreet;
                    break;
            }

            url = url.Replace("{z}", zoomLevel.ToString());
            url = url.Replace("{x}", tilePositionX.ToString());
            url = url.Replace("{y}", y.ToString());

            return new Uri(url);

        }


        /// <summary>
        /// Return Map Modes.
        /// </summary>
        public override string[] MapModes
        {
            get
            {
                return System.Enum.GetNames(typeof(YhooMapModes));
            }
        }

        /// <summary>
        /// Switches Map Mode.
        /// </summary>
        /// <param name="NewMode"></param>
        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (YhooMapModes)Enum.Parse(typeof(YhooMapModes), NewMode);
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
