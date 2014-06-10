using System;
using System.Windows.Media.Media3D;
using WPF.Globe.ClientControl.Models;

namespace WPF.Globe.ClientControl.GIS
{
    public class GISHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadToDeg(double radians)
        {
            return ((radians / 3.1415926535897931) * 180.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegToRad(double degrees)
        {
            return ((degrees / 180.0) * Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double MercatorYToLatitude(double y)
        {
            return ((2.0 * Math.Atan(Math.Exp(y))) - 1.5707963267948966);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phi"></param>
        /// <returns></returns>
        public static double LatitudeToMercatorY(double phi)
        {
            double positiveInfinity;
            if ((0.78539816339744828 + (0.5 * phi)) == 1.5707963267948966)
            {
                positiveInfinity = double.PositiveInfinity;
            }
            else
            {
                positiveInfinity = Math.Tan(0.78539816339744828 + (0.5 * phi));
            }
            return Math.Round(Math.Log(positiveInfinity), 15);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static LatLong PointToLatLong(Point3D point, AxisAngleRotation3D rotation)
        {
            Point3D pointd = ViewPointToSpherePoint(point, rotation);
            double d = Math.Asin(pointd.Y);
            double radians = Math.Acos(-pointd.X / Math.Cos(d));
            d = RadToDeg(d);
            radians = RadToDeg(radians);
            if (pointd.Z < 0.0)
            {
                radians = 180.0 + (180.0 - radians);
            }
            radians = (radians + 270.0) % 360.0;
            return new LatLong(d, radians - 180.0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewPoint"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Point3D ViewPointToSpherePoint(Point3D viewPoint, AxisAngleRotation3D rotation)
        {
            return ViewPointToSpherePoint(viewPoint, false, rotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewPoint"></param>
        /// <param name="reverse"></param>
        /// <param name="_rotation"></param>
        /// <returns></returns>
        public static Point3D ViewPointToSpherePoint(Point3D viewPoint, bool reverse, AxisAngleRotation3D _rotation)
        {
            Quaternion quaternion = new Quaternion(viewPoint.X, viewPoint.Y, viewPoint.Z, 0.0);
            Quaternion quaternion2 = reverse
                                         ? new Quaternion(_rotation.Axis, -_rotation.Angle)
                                         : new Quaternion(_rotation.Axis, _rotation.Angle);
            Quaternion quaternion3 = quaternion2;
            quaternion3.Conjugate();
            Quaternion quaternion4 = (quaternion2 * quaternion) * quaternion3;
            new Quaternion(1.0, 1.0, 1.0 / quaternion4.Z, 1.0);
            return new Point3D(quaternion4.X, quaternion4.Y, quaternion4.Z);
        }

    }
}
