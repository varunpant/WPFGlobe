using System;
namespace WPF.Globe.ClientControl.Models
{
    public class ZoomManager
    {

        public static double[] ZoomLevelMetersPerPixel = new[] {
                                                           39135.76,
                                                           19567.88,
                                                           9783.94,
                                                           4891.97,
                                                           2445.98,
                                                           1222.99,
                                                           611.5,
                                                           305.75,
                                                           152.87,
                                                           76.44,
                                                           38.22,
                                                           19.11,
                                                           9.55,
                                                           4.78,
                                                           2.39,
                                                           1.19,
                                                           0.6,
                                                           0.3
                                                       };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static long ZoomLevelColumns(int ZoomLevel)
        {
            return (long)Math.Sqrt((double)ZoomLevelImages(ZoomLevel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static long ZoomLevelRows(int ZoomLevel)
        {
            return (long)Math.Sqrt((double)ZoomLevelImages(ZoomLevel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static long ZoomLevelImages(int ZoomLevel)
        {
            return (long)Math.Pow(4.0, (double)ZoomLevel);
        }


      
    }
}
