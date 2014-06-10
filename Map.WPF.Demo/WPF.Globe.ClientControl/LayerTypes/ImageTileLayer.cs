using System;
using WPF.Globe.ClientControl.Models;
using WPF.Globe.ClientControl.GIS;

namespace WPF.Globe.ClientControl.LayerTypes
{
    public abstract class ImageTileLayer : ILayer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public abstract string GetImageUrl(int column, int row, int zoomLevel);

        /// <summary>
        /// Returns quadkey based on col, row, zoom [Bing tiling scheme]
        /// </summary>
        /// <returns></returns>
        public static string GetVENumber(int column, int row, int zoomLevel)
        {
            string str = "";
            double num = 0.0;
            double num2 = column + 1;
            double num3 = row + 1;
            for (int i = zoomLevel; i > 0; i--)
            {
                int num4 = 0;
                if (num2 > (ZoomManager.ZoomLevelColumns(zoomLevel) * Math.Pow(0.5, (double)((zoomLevel - i) + 1))))
                {
                    num4++;
                    num += 1.0 * Math.Pow(10.0, (double)(i - 1));
                    num2 -= Math.Pow(2.0, (double)(i - 1));
                }
                if (num3 > (ZoomManager.ZoomLevelColumns(zoomLevel) * Math.Pow(0.5, (double)((zoomLevel - i) + 1))))
                {
                    num4 += 2;
                    num += 2.0 * Math.Pow(10.0, (double)(i - 1));
                    num3 -= Math.Pow(2.0, (double)(i - 1));
                }
                str = str + num4.ToString();
            }
            string str2 = str;
            while (str2.Length < zoomLevel)
            {
                str2 = "0" + str2;
            }
            if (str2.Length == 0)
            {
                str2 = "0";
            }
            return str2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quadKey"></param>
        /// <returns></returns>
        public static BBox QuadKeyToBBox(string quadKey)
        {
            const int x = 0;
            const int y = 262144;
            return QuadKeyToBBox(quadKey, x, y, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quadKey"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public static BBox QuadKeyToBBox(string quadKey, int x, int y, int zoomLevel)
        {
            char c = quadKey[0];

            int tileSize = 2 << (18 - zoomLevel - 1);

            if (c == '0')
            {
                y = y - tileSize;
            }

            else if (c == '1')
            {
                y = y - tileSize;
                x = x + tileSize;
            }

            else if (c == '3')
            {
                x = x + tileSize;
            }

            if (quadKey.Length > 1)
            {
                return QuadKeyToBBox(quadKey.Substring(1), x, y, zoomLevel + 1);
            }
            return new BBox(x, y, tileSize, tileSize);
        }

        /// <summary>
        /// Converts a QuadKey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        public static void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            tileX = tileY = 0;
            levelOfDetail = quadKey.Length;
            for (int i = levelOfDetail; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                switch (quadKey[levelOfDetail - i])
                {
                    case '0':
                        break;

                    case '1':
                        tileX |= mask;
                        break;

                    case '2':
                        tileY |= mask;
                        break;

                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;

                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public abstract string[] MapModes { get;  }

        /// <summary>
        /// Changes Map Mode.
        /// </summary>
        /// <param name="NewMode"></param>
        public abstract void SwitchMapMode(string NewMode);

        /// <summary>
        /// 
        /// </summary>
        public abstract string CurrentMapMode{ get;}
       
    }
}
