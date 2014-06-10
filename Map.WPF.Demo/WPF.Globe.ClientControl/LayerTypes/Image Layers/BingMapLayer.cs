
using System;
namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class BingMapLayer : ImageTileLayer
    {
        /// <summary>
        /// Image Modes.
        /// </summary>
        public enum ImageMode
        {
            h,
            a,
            r
        }

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

            if (MapMode.ToString() == "r")
            {
                return string.Concat(new object[] { "http://r", code[code.Length - 1], ".ortho.tiles.virtualearth.net/tiles/r", code, ".png?g=1" });
            }

            return string.Concat(new object[] { "http://", MapMode.ToString(), code[code.Length - 1], ".ortho.tiles.virtualearth.net/tiles/", MapMode.ToString(), code, ".jpeg?g=1" });

        }

        /// <summary>
        /// Current Image Mode.
        /// </summary>
        public ImageMode MapMode { get; set; }

        /// <summary>
        /// Return Map Modes.
        /// </summary>
        public override string[] MapModes
        {
            get
            {
                return System.Enum.GetNames(typeof(ImageMode));
            }           
        }

        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (ImageMode)Enum.Parse(typeof(ImageMode), NewMode);
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
