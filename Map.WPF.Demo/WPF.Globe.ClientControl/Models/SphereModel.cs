using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF.Globe.ClientControl.Models
{
    public class SphereModel : UIElement3D
    {
        /// <summary>
        /// OnUpdateModel is called in response to InvalidateModel and provides
        /// a place to set the Visual3DModel property
        /// 
        /// Setting Visual3DModel will not link up parenting information which
        /// is needed for databinding, styling, etc... just as creating render data
        /// in the 2D world won't cause this as well.
        /// 
        /// To get this properly linked up, we create a Model dependency property which itself
        /// sets this value.  The Model DP then causes the correct linkup to occur
        /// and the above features to work correctly.
        /// 
        /// In this update model we retessellate the sphere based on the current
        /// dependency property values, and then set it as the model.  The brush
        /// color is blue by default, but the code could easily be updated to let
        /// this be set by the user.
        /// </summary>
        protected override void OnUpdateModel()
        {
            GeometryModel3D model = new GeometryModel3D();

            model.Geometry = Tessellate(ThetaDiv, PhiDiv, Radius);
            model.Material = new DiffuseMaterial(Brushes.Blue);

            Model = model;
        }

        /// <summary>
        /// The Model property for the sphere
        /// </summary>
        private static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model",
                                        typeof(Model3D),
                                        typeof(SphereModel),
                                        new PropertyMetadata(ModelPropertyChanged));

        private static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SphereModel s = (SphereModel)d;
            s.Visual3DModel = (Model3D)e.NewValue;
        }

        private Model3D Model
        {
            get
            {
                return (Model3D)GetValue(ModelProperty);
            }

            set
            {
                SetValue(ModelProperty, value);
            }
        }

        /// <summary>
        /// The number of vertical slices to make on the SphereModel
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty =
            DependencyProperty.Register("ThetaDiv",
                                        typeof(int),
                                        typeof(SphereModel),
                                        new PropertyMetadata(15, ThetaDivPropertyChanged));

        private static void ThetaDivPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SphereModel s = (SphereModel)d;
            s.InvalidateModel();
        }

        public int ThetaDiv
        {
            get
            {
                return (int)GetValue(ThetaDivProperty);
            }

            set
            {
                SetValue(ThetaDivProperty, value);
            }
        }

        /// <summary>
        /// The number of horizontal slices to make on the SphereModel
        /// </summary>
        public static readonly DependencyProperty PhiDivProperty =
            DependencyProperty.Register("PhiDiv",
                                        typeof(int),
                                        typeof(SphereModel),
                                        new PropertyMetadata(15, PhiDivPropertyChanged));

        private static void PhiDivPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SphereModel s = (SphereModel)d;
            s.InvalidateModel();
        }

        public int PhiDiv
        {
            get
            {
                return (int)GetValue(PhiDivProperty);
            }

            set
            {
                SetValue(PhiDivProperty, value);
            }
        }

        /// <summary>
        /// The radius of the sphere
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius",
                                        typeof(double),
                                        typeof(SphereModel),
                                        new PropertyMetadata(1.0, RadiusPropertyChanged));

        private static void RadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SphereModel s = (SphereModel)d;
            s.InvalidateModel();
        }

        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        /// <summary>
        /// Private helper methods
        /// </summary>
        private static Point3D GetPosition(double theta, double phi, double radius)
        {
            double x = radius * Math.Sin(theta) * Math.Sin(phi);
            double y = radius * Math.Cos(phi);
            double z = radius * Math.Cos(theta) * Math.Sin(phi);

            return new Point3D(x, y, z);
        }

        private static Vector3D GetNormal(double theta, double phi)
        {
            return (Vector3D)GetPosition(theta, phi, 1.0);
        }

        private static double DegToRad(double degrees)
        {
            return (degrees / 180.0) * Math.PI;
        }

        private static Point GetTextureCoordinate(double theta, double phi)
        {
            Point p = new Point(theta / (2 * Math.PI),
                                phi / (Math.PI));

            return p;
        }

        /// <summary>
        /// Tessellates the sphere and returns a MeshGeometry3D representing the 
        /// tessellation based on the given parameters
        /// </summary>
        /// <param name="tDiv"></param>
        /// <param name="pDiv"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        internal static MeshGeometry3D Tessellate(int tDiv, int pDiv, double radius)
        {
            double dt = DegToRad(360.0) / tDiv;
            double dp = DegToRad(180.0) / pDiv;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int pi = 0; pi <= pDiv; pi++)
            {
                double phi = pi * dp;

                for (int ti = 0; ti <= tDiv; ti++)
                {
                    // we want to start the mesh on the x axis
                    double theta = ti * dt;

                    mesh.Positions.Add(GetPosition(theta, phi, radius));
                    mesh.Normals.Add(GetNormal(theta, phi));
                    mesh.TextureCoordinates.Add(GetTextureCoordinate(theta, phi));
                }
            }

            for (int pi = 0; pi < pDiv; pi++)
            {
                for (int ti = 0; ti < tDiv; ti++)
                {
                    int x0 = ti;
                    int x1 = (ti + 1);
                    int y0 = pi * (tDiv + 1);
                    int y1 = (pi + 1) * (tDiv + 1);

                    mesh.TriangleIndices.Add(x0 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y0);

                    mesh.TriangleIndices.Add(x1 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y1);
                }
            }

            mesh.Freeze();
            return mesh;
        }
    }
}
