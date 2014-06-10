namespace WPF.Globe.ClientControl.Models
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct LatLong
    {
        public double Latitude;
        public double Longitude;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lat"></param>
        /// <param name="Lon"></param>
        public LatLong(double Lat, double Lon)
        {
            Latitude = Lat;
            Longitude = Lon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (Latitude + ";" + Longitude);
        }
    }
}
