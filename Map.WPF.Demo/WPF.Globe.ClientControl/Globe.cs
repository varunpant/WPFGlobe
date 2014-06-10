using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using WPF.Globe.ClientControl.GIS;
using WPF.Globe.ClientControl.Models;
using System.Collections.Generic;

namespace WPF.Globe.ClientControl
{
    public class Globe : Control
    {

        #region Private

        private Point _previousPosition2D;
        private Vector3D _previousPosition3D = new Vector3D(0.0, 0.0, 1.0);

        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();
        private RotateTransform3D _rotationTranform;

        private ScaleTransform3D _scale = new ScaleTransform3D();
        private Transform3DGroup _transform;

        private FrameworkElement EventSource;

        private GlobeTile RootTile;

        #endregion

        #region Public Properties

        //Base Layer.
        public static ILayer BaseLayer
        {
            get;
            set;
        }

        /// <summary>
        /// Current Zoom Level of Globe.
        /// </summary>
        public int CurrentZoomLevel { get; private set; }

        public delegate void ZoomLevelChangedHandler(object Sender, int newLevel);
        public event ZoomLevelChangedHandler ZoomLevelChanged;



        #endregion

        #region Constructor

        static Globe()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Globe), new FrameworkPropertyMetadata(typeof(Globe)));
        }

        Viewport3D _viewport;

        Viewport3D ViewPort
        {
            get
            {
                if (_viewport == null)
                {
                    _viewport = new Viewport3D();

                    PerspectiveCamera PCamera = new PerspectiveCamera();

                    // Specify where in the 3D scene the camera is.
                    PCamera.Position = new Point3D(0, 0, 1);

                    // Specify the direction that the camera is pointing.
                    PCamera.LookDirection = new Vector3D(0, 0, -1);

                    // Specify the Up direction that the camera is pointing.
                    PCamera.UpDirection = new Vector3D(0, 1, 0);

                    // Define camera's horizontal field of view in degrees.
                    PCamera.FieldOfView = 0.000001;

                    // Asign the camera to the viewport
                    _viewport.Camera = PCamera;

                    Border Space = GetTemplateChild("PART_Space") as Border;
                    Space.Child = _viewport;
                }

                return _viewport;

            }
        }     

        /// <summary>
        /// 
        /// </summary>
        public void Initialise(ILayer baseLayer)
        {
            #region Construct View Port , Add Transforms, Scale

            Viewport3D viewport = ViewPort;
            _transform = new Transform3DGroup();
            _transform.Children.Add(_scale);
            _rotationTranform = new RotateTransform3D(_rotation);
            _transform.Children.Add(_rotationTranform);

            AmbientLight _ambLight = new AmbientLight(System.Windows.Media.Brushes.White.Color);
            var visuald = new ModelVisual3D();
            Model3DGroup M3dG = new Model3DGroup();
            M3dG.Children.Add(_ambLight);
            visuald.Content = M3dG;
            viewport.Children.Add(visuald);

            #endregion

            BaseLayer = baseLayer;
            this.RootTile = new GlobeTile(0, 0, 0, visuald);

            var root = this.RootTile;
            this.RootTile.zoomIn();

            //Map Viewport as Event Source to Map Event Handlers.
            EventSource = viewport;

            //Add All Necessary Transforms to view port Camera.
            viewport.Camera.Transform = _transform;

            #region Populate Initial Layer [Based on MS Bing Map]


            var initialKey = "032010110132012031";//initial key
            for (var i = 0; i < initialKey.Length; i++)
            {
                root.Children[int.Parse(initialKey.Substring(i, 1))].zoomIn();
            }


            #endregion

            #region Map Events to Handelers

            viewport.MouseDown += viewport_MouseDown;
            viewport.MouseUp += viewport_MouseUp;
            viewport.MouseMove += viewport_MouseMove;
            viewport.MouseWheel += viewport_MouseWheel;
            viewport.SizeChanged += viewport_SizeChanged;

            #endregion

            RedrawSphere();
        }

        #endregion

        #region Geometry Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="latlong"></param>
        /// <returns></returns>
        private static Point3D LatLongToPoint(LatLong latlong)
        {
            double latitude = latlong.Latitude;
            double degrees = latlong.Longitude - 90.0;
            Point3D pointd = new Point3D();
            latitude = GISHelper.DegToRad(latitude);
            degrees = GISHelper.DegToRad(degrees);
            pointd.Y = Math.Sin(latitude);
            pointd.X = -Math.Cos(degrees) * Math.Cos(latitude);
            pointd.Z = Math.Sin(degrees) * Math.Cos(latitude);
            return pointd;
        }

        /// <summary>
        /// Converts Globe Point to Lat Long.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        LatLong PointToLatLong(Point3D point)
        {
            Point3D pointd = ViewPointToSpherePoint(point);
            double d = Math.Asin(pointd.Y);
            double radians = Math.Acos(-pointd.X / Math.Cos(d));
            d = GISHelper.RadToDeg(d);
            radians = GISHelper.RadToDeg(radians);
            if (pointd.Z < 0.0)
            {
                radians = 180.0 + (180.0 - radians);
            }
            radians = (radians + 270.0) % 360.0;
            return new LatLong(d, radians - 180.0);
        }

        /// <summary>
        /// Maps View Point in 2D to Sphere point [Microsoft Wpf Blog.]
        /// </summary>
        /// <param name="viewPoint"></param>
        /// <returns></returns>
        Point3D ViewPointToSpherePoint(Point3D viewPoint)
        {
            return ViewPointToSpherePoint(viewPoint, false);
        }

        /// <summary>
        /// Maps 2d Point to Spehere Point.
        /// </summary>
        /// <param name="viewPoint"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        Point3D ViewPointToSpherePoint(Point3D viewPoint, bool reverse)
        {
            Quaternion quaternion = new Quaternion(viewPoint.X, viewPoint.Y, viewPoint.Z, 0.0);
            Quaternion quaternion2 = reverse
                                         ? new Quaternion(_rotation.Axis, -_rotation.Angle)
                                         : new Quaternion(_rotation.Axis, _rotation.Angle);
            Quaternion quaternion3 = quaternion2;
            quaternion3.Conjugate();
            Quaternion quaternion4 = (quaternion2 * quaternion) * quaternion3;
            // new Quaternion(1.0, 1.0, 1.0 / quaternion4.Z, 1.0);
            return new Point3D(quaternion4.X, quaternion4.Y, quaternion4.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double sphereWidth = GetSphereWidth();
            double x = point.X - ((width - sphereWidth) / 2.0);
            double y = point.Y - ((height - sphereWidth) / 2.0);
            point = new Point(x, y);
            width = height = sphereWidth;
            double ar = width / height;
            point.Y *= ar;
            height = width;
            double num5 = point.X / (width / 2.0);
            double num6 = point.Y / (height / 2.0);
            num5--;
            num6 = 1.0 - num6;
            double d = (1.0 - (num5 * num5)) - (num6 * num6);
            return new Vector3D(num5, num6, (d > 0.0) ? Math.Sqrt(d) : 0.0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void EnsureZoom()
        {
            var center = GetCenter();
            RootTile.EnsureZoom(180.0 + center.Longitude, 90.0 - center.Latitude, CurrentZoomLevel + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LatLong GetCenter()
        {
            return PointToLatLong(new Point3D(0.0, 0.0, 1.0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double GetSphereWidth()
        {
            var extent = ZoomManager.ZoomLevelMetersPerPixel[CurrentZoomLevel];
            var EarthRadius = 6378135.0;
            var globeRadious = (EarthRadius * 3.1415926535897931) / 2.0;
            return (globeRadious / extent);
        }

        /// <summary>
        /// 
        /// </summary>
        void RedrawSphere()
        {
            double extent = ZoomManager.ZoomLevelMetersPerPixel[CurrentZoomLevel];
            double EarthRadius = 6378135.0;
            double num1 = (EarthRadius * 3.1415926535897931) / 2.0;
            Viewport3D viewport = ViewPort;
            double num3 = viewport.ActualWidth / ((2.0 * EarthRadius) / extent);
            double num4 = 1.15470054;
            double num5 = num4 * num3;
            double num6 = num5 / Math.Tan(GISHelper.DegToRad(5E-07));
            _scale.ScaleX = num6;
            _scale.ScaleY = num6;
            _scale.ScaleZ = num6;
        }

        /// <summary>
        /// Current Mouse Position on spere.
        /// </summary>
        /// <param name="CurrentPosition"></param>
        void Track(Point CurrentPosition)
        {
            Vector3D vectord = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, CurrentPosition);
            Track(vectord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrentPosition3D"></param>
        void Track(Vector3D CurrentPosition3D)
        {
            Vector3D axisOfRotation = Vector3D.CrossProduct(_previousPosition3D, CurrentPosition3D);
            double rotationAngle = Vector3D.AngleBetween(_previousPosition3D, CurrentPosition3D);
            Quaternion quaternion = new Quaternion(axisOfRotation, -rotationAngle);
            Quaternion quaternion2 = new Quaternion(_rotation.Axis, _rotation.Angle);
            quaternion2 *= quaternion;

            _rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);//new DoubleAnimation(0, 130d, TimeSpan.FromMilliseconds(3000))
            _rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, null);

            _rotation.Axis = quaternion2.Axis;
            _rotation.Angle = quaternion2.Angle;
            _previousPosition3D = CurrentPosition3D;

        }

        #endregion

        #region Event Handelers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void viewport_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.Element);
            _previousPosition2D = e.GetPosition(EventSource);
            _previousPosition3D = ProjectToTrackball(EventSource.ActualWidth, EventSource.ActualHeight, _previousPosition2D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void viewport_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point position = e.GetPosition(EventSource);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Track(position);
            }
            _previousPosition2D = position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void viewport_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.None);
            EnsureZoom();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void viewport_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if ((e.Delta > 0) && (CurrentZoomLevel < 0x11))
            {
                CurrentZoomLevel++; if (ZoomLevelChanged != null) ZoomLevelChanged(this, CurrentZoomLevel);
                RedrawSphere();
            }
            else if ((e.Delta < 0) && (CurrentZoomLevel >= 1))
            {
                CurrentZoomLevel--; if (ZoomLevelChanged != null) ZoomLevelChanged(this, CurrentZoomLevel);
                RedrawSphere();
            }


            EnsureZoom();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawSphere();
        }

        #endregion

        #region Animation

        /// <summary>
        /// not in use yet
        /// </summary>
        void rotateAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.0;
            animation.To = 360.0;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500));
            animation.RepeatBehavior = new RepeatBehavior(3.0);
            _rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void RotateBetween(Vector3D from, Vector3D to)
        {
            Vector3D axisOfRotation = Vector3D.CrossProduct(from, to);
            double num = Vector3D.AngleBetween(from, to);
            Quaternion quaternion = new Quaternion(axisOfRotation, -num);
            Quaternion quaternion2 = new Quaternion(_rotation.Axis, _rotation.Angle);
            quaternion2 *= quaternion;
            new Storyboard();
            Vector3DAnimation animation = new Vector3DAnimation(_rotation.Axis, quaternion2.Axis,
                                                                new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            DoubleAnimation animation2 = new DoubleAnimation(_rotation.Angle, quaternion2.Angle,
                                                             new Duration(new TimeSpan(0, 0, 0, 0, 500)));
            _rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation2);
            _rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, animation);
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Redraws/Refreshes Globe.
        /// </summary>
        public void GoToGlobePosition(double Latitude, double Longitude)
        {
            LatLong latlong = new LatLong(Latitude, Longitude);
            Point3D pointd = ViewPointToSpherePoint(LatLongToPoint(latlong), true);
            RotateBetween(new Vector3D(pointd.X, pointd.Y, pointd.Z), new Vector3D(0.0, 0.0, 1.0));
        }

        /// <summary>
        /// Zooms Globe.
        /// </summary>
        /// <param name="ZoomLevel"></param>
        public void ZoomGlobe(int ZoomLevel)
        {
            if (CurrentZoomLevel != ZoomLevel)
            {
                CurrentZoomLevel = ZoomLevel;
                RedrawSphere();
                EnsureZoom();
            }
        }

        /// <summary>
        /// Changes the base Layer of the globe.
        /// </summary>
        /// <param name="layer"></param>
        public void SwitchBaseLayer(ILayer layer)
        {
            BaseLayer = layer;
            RootTile.Refresh();
            RedrawSphere();
        }

        /// <summary>
        /// Chnages the map Mode.
        /// </summary>
        /// <param name="PossibleMode"></param>
        public void SwitchMapMode(string PossibleMode)
        {
            ILayer Layer = BaseLayer;
            if (!Layer.CurrentMapMode.Equals(PossibleMode))
            {
                Layer.SwitchMapMode(PossibleMode);
                RootTile.Refresh();
                RedrawSphere();
            }
        }

        #endregion

    }
}
