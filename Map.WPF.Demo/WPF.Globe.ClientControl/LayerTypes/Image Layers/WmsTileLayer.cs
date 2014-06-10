

using WPF.Globe.ClientControl.GIS;
using System;
using WPF.Globe.ClientControl.Models;
namespace WPF.Globe.ClientControl.LayerTypes.Image_Layers
{
    public class WmsTileLayer : ImageTileLayer
    {
        public enum WMSPaths
        {
            metacarta,
            DemisWMS,
            OnEarthLandsatWMS,
            NASAGlobalMosaic,
            CivilMapsTileEngine,
            DMSolutionsGroup,
            PBBI
        }

        string TilePath = string.Empty;
 

        public override string GetImageUrl(int column, int row, int zoomLevel)
        {
            GenerateTilePath();
            string quadKey = (GetVENumber(column, row, zoomLevel));
            BBox boundingBox = QuadKeyToBBox(quadKey);

            // Get the lat longs of the corners of the box
            double lon = XToLongitudeAtZoom(boundingBox.x * 256, 18);
            double lat = YToLatitudeAtZoom(boundingBox.y * 256, 18);

            double lon2 = XToLongitudeAtZoom((boundingBox.x + boundingBox.width) * 256, 18);
            double lat2 = YToLatitudeAtZoom((boundingBox.y - boundingBox.height) * 256, 18);

            string wmsUrl = string.Format(TilePath, lon, lat, lon2, lat2, 256);

            return new Uri(wmsUrl).AbsoluteUri;

        }

        private void GenerateTilePath()
        {
            switch (MapMode)
            {
                case WMSPaths.metacarta:
                    {
                        TilePath = @"http://labs.metacarta.com/wms/vmap0?LAYERS=basic&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&EXCEPTIONS=application/vnd.ogc.se_inimage&FORMAT=image/jpeg&SRS=EPSG:4326&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={4}";
                    }
                    break;
                case WMSPaths.DemisWMS:
                    {
                        TilePath = @"http://www2.demis.nl/wms/wms.asp?wms=WorldMap&LAYERS=Coastlines&FORMAT=image/png&VERSION=1.1.1&SERVICE=WMS&REQUEST=GetMap&STYLES=&EXCEPTIONS=application/vnd.ogc.se_inimage&SRS=EPSG:4326&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={4}";
                    }
                    break;
                case WMSPaths.OnEarthLandsatWMS:
                    {
                        TilePath = @"http://onearth.jpl.nasa.gov/wms.cgi?request=GetMap&width=128&height=128&layers=modis&styles=&srs=EPSG:4326&format=image/png&bbox={0},{1},{2},{3}";
                    }
                    break;
                case WMSPaths.NASAGlobalMosaic:
                    {
                        TilePath = @"http://t1.hypercube.telascience.org/cgi-bin/landsat7?LAYERS=landsat7&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&EXCEPTIONS=application/vnd.ogc.se_inimage&FORMAT=image/jpeg&SRS=EPSG:4326&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={4}";
                    }
                    break;
                case WMSPaths.CivilMapsTileEngine:
                    {
                        TilePath = @"http://maps.civicactions.net/cgi-bin/mapserv?map=/www/sites/maps.civicactions.net/maps/world.map&service=WMS&WMTVER=1.0.0&REQUEST=map&SRS=EPSG:4326&LAYERS=bluemarble,landsat7,lakes,rivers,cities,majorroads,minorroads,tiger_polygon,tiger_landmarks,tiger_lakes,tiger_local_roads,tiger_major_roads,lowboundaries,boundaries,coastlines&FORMAT=image/jpeg&STYLES=&TRANSPARENT=TRUE&WIDTH={4}&HEIGHT=128&BBOX={0},{1},{2},{3}";
                    }
                    break;
                case WMSPaths.DMSolutionsGroup:
                    {
                        TilePath = @"http://www.mapsherpa.com/cgi-bin/wms_iodra?LAYERS=Bathymetry&FORMAT=image%2Fpng&VERSION=1.1.1&SERVICE=WMS&REQUEST=GetMap&STYLES=&EXCEPTIONS=application%2Fvnd.ogc.se_inimage&SRS=EPSG%3A4326&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={4}";
                    }
                    break;
                case WMSPaths.PBBI:
                    {
                        TilePath = @"http://kansingh-w1:8102/WMSService/WMS?SERVICE=WMS&REQUEST=GetMap&SRS=EPSG:4326&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={4}&STYLES=&FORMAT=image/png&Layers=Buildings";
                    }
                    break;


            }
        }


        public const double EarthCircumference = 6378137 * 2 * Math.PI;
        public const double HalfEarthCircumference = EarthCircumference / 2;

        /// <summary>
        /// Converts a grid row to Latitude
        /// </summary>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static double YToLatitudeAtZoom(int y, int zoom)
        {
            double arc = EarthCircumference / ((1 << zoom) * 256);
            double metersY = HalfEarthCircumference - (y * arc);
            double a = Math.Exp(metersY * 2 / 6378137);
            double result = GISHelper.RadToDeg(Math.Asin((a - 1) / (a + 1)));
            return result;
        }

        /// <summary>
        /// Converts a grid column to Longitude
        /// </summary>
        /// <param name="x"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static double XToLongitudeAtZoom(int x, int zoom)
        {
            double arc = EarthCircumference / ((1 << zoom) * 256);
            double metersX = (x * arc) - HalfEarthCircumference;
            double result = GISHelper.RadToDeg(metersX / 6378137);
            return result;
        }


        /// <summary>
        /// Return Map Modes.
        /// </summary>
        public override string[] MapModes
        {
            get
            {
                return System.Enum.GetNames(typeof(WMSPaths));
            }
        }

        /// <summary>
        /// Current Image Mode.
        /// </summary>
        public WMSPaths MapMode { get; set; }

        /// <summary>
        /// Switches Map Mode.
        /// </summary>
        /// <param name="NewMode"></param>
        public override void SwitchMapMode(string NewMode)
        {
            MapMode = (WMSPaths)Enum.Parse(typeof(WMSPaths), NewMode);
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
