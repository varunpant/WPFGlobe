using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WPF.Globe.ClientControl.GIS;
using WPF.Globe.ClientControl.LayerTypes;

namespace WPF.Globe.ClientControl.Models
{
    public class GlobeTile
    {
        #region Public properties

        /// <summary>
        /// Tile Children Tree.
        /// </summary>
        public GlobeTile[] Children { get; set; }

        /// <summary>
        /// Tile Parent.
        /// </summary>
        public GlobeTile Parent { get; set; }

        /// <summary>
        /// Tile Row.
        /// </summary>
        int Column;

        /// <summary>
        /// Tile Column.
        /// </summary>
        int Row;

        /// <summary>
        /// Provides services and properties that are common to all visual objects, including hit-testing, coordinate transformation,
        /// and bounding-box calculations.
        /// The ModelVisual3D class has a Children property that enables you to build a tree structure of ModelVisual3D objects. 
        /// </summary>
        ModelVisual3D Container;

        /// <summary>
        /// 
        /// </summary>
        bool Incontainer;

        /// <summary>
        /// 
        /// </summary>
        ModelVisual3D SphereModel;

        /// <summary>
        /// 
        /// </summary>
        double phiEnd;

        /// <summary>
        /// 
        /// </summary>
        double phiStart;

        /// <summary>
        /// 
        /// </summary>
        int ZoomLevel;

        #endregion

        #region Constructor and overloads

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ZoomLevel"></param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="Container"></param>
        public GlobeTile(int ZoomLevel, int Row, int Column, ModelVisual3D Container)
        {
            this.phiStart = double.NaN;
            this.phiEnd = double.NaN;
            this.ZoomLevel = ZoomLevel;
            this.Row = Row;
            this.Column = Column;
            this.Container = Container;
            this.SphereModel = GetModelVisual3D();
            this.SetMaterial();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="mode"></param>
        /// <param name="container"></param>
        /// <param name="parent"></param>
        public GlobeTile(int ZoomLevel, int Row, int Column, ModelVisual3D Container, GlobeTile Parent)
        {
            this.phiStart = double.NaN;
            this.phiEnd = double.NaN;
            this.ZoomLevel = ZoomLevel;
            this.Row = Row;
            this.Column = Column;
            this.Container = Container;
            this.Parent = Parent;
            this.SphereModel = GetModelVisual3D();
            this.SetMaterial();
        }

        #endregion

        #region Populating children tiles of this tile. [Bing VE Tiling Scheme.]

        /// <summary>
        /// 
        /// </summary>
        public void PopulateChildrenTiles()
        {
            if (this.Children == null)
            {
                this.Children = new GlobeTile[4];
                for (int j = 0; j < 4; j++)
                {
                    //Calculation of children tiles based on current tile. [ ref: Bing Tile Scheme ]. 
                    int rowOffset = (int)Math.Floor((double)(j / 2));
                    int collOffset = (int)Math.Floor((double)(j % 2));

                    this.Children[j] = new GlobeTile(this.ZoomLevel + 1, (this.Row * 2) + rowOffset, (this.Column * 2) + collOffset, this.Container, this);
                }
            }

        }

        /// <summary>
        /// Adds current tile to Container.
        /// </summary>
        public void AddToContainer()
        {
            if (this.Children != null)
            {
                foreach (GlobeTile tile in this.Children)
                {
                    tile.RemoveFromContainer();
                }
                this.Children = null;
            }
            if (!this.Incontainer)
            {
                this.Incontainer = true;
                this.Container.Children.Add(this.SphereModel);
            }
        }

        /// <summary>
        /// Removes this tile from container.
        /// </summary>
        public void RemoveFromContainer()
        {
            this.Incontainer = false;
            this.Container.Children.Remove(this.SphereModel);
        }

        /// <summary>
        /// Repaints the tiles with material.
        /// </summary>
        public void Refresh()
        {           
            if (this.Children != null)
            {
                foreach (GlobeTile tile in this.Children)
                {
                    tile.Refresh();
                }
            }

            this.SetMaterial();
        }

        #endregion

        #region Zoom

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="toZoomLevel"></param>
        public void EnsureZoom(double x, double y, int toZoomLevel)
        {
            if (this.ZoomLevel < toZoomLevel)
            {
                this.PopulateChildrenTiles();
                int num = (int)Math.Floor((double)(x / 180.0));
                int num2 = 0;
                if (num == 2)
                {
                    num--;
                }
                if (num2 == 2)
                {
                    num2--;
                }
                x = (num == 0) ? (x * 2.0) : ((x - 180.0) * 2.0);
                if (this.Children[0].GetPhiEnd() < y)
                {
                    num2 = 1;
                }
                int index = num + (2 * num2);
                this.RemoveFromContainer();
                for (int i = 0; i < 4; i++)
                {
                    if (i != index)
                    {
                        this.Children[i].TryZoom(toZoomLevel);
                    }
                }
                this.Children[index].EnsureZoom(x, y, toZoomLevel);
            }
            else if ((this.ZoomLevel <= toZoomLevel) && (this.ZoomLevel == toZoomLevel))
            {
                this.AddToContainer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toZoomLevel"></param>
        public void TryZoom(int toZoomLevel)
        {
            if (this.ZoomLevel == toZoomLevel)
            {
                this.AddToContainer();
            }
            else if (this.Children != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.Children[i].TryZoom(toZoomLevel);
                }
            }
            else
            {
                this.AddToContainer();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void zoomIn()
        {
            if (this.Children == null)
            {
                this.PopulateChildrenTiles();
            }
            for (int i = 0; i < 4; i++)
            {
                this.Children[i].AddToContainer();
            }
        }

        #endregion

        #region Geometry Helper


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        double GetPhiEnd()
        {
            if (double.IsNaN(this.phiEnd))
            {
                double num = 7.1288559127654789;
                double num3 = (GISHelper.LatitudeToMercatorY(GISHelper.DegToRad(45.0)) * num) / 2.0;
                double num4 = -num3;
                double num5 = num3 - num4;
                double num1 = ((double)this.Row) / ((double)ZoomManager.ZoomLevelRows(this.ZoomLevel));
                double num6 = (this.Row + 1.0) / ((double)ZoomManager.ZoomLevelRows(this.ZoomLevel));
                GISHelper.RadToDeg(GISHelper.MercatorYToLatitude(1.0));
                ZoomManager.ZoomLevelRows(this.ZoomLevel);
                double y = (num6 - 0.5) * num5;
                this.phiEnd = GISHelper.RadToDeg(GISHelper.MercatorYToLatitude(y)) + 90.0;
            }
            return this.phiEnd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        double GetPhiStart()
        {
            if (double.IsNaN(this.phiStart))
            {
                double num = 7.1288559127654789;
                double num3 = (GISHelper.LatitudeToMercatorY(GISHelper.DegToRad(45.0)) * num) / 2.0;
                double num4 = -num3;
                double num5 = num3 - num4;
                double num6 = ((double)this.Row) / ((double)ZoomManager.ZoomLevelRows(this.ZoomLevel));
                double num1 = (this.Row + 1.0) / ((double)ZoomManager.ZoomLevelRows(this.ZoomLevel));
                GISHelper.RadToDeg(GISHelper.MercatorYToLatitude(1.0));
                ZoomManager.ZoomLevelRows(this.ZoomLevel);
                double y = (num6 - 0.5) * num5;
                this.phiStart = GISHelper.RadToDeg(GISHelper.MercatorYToLatitude(y)) + 90.0;
            }
            return this.phiStart;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Theta"></param>
        /// <param name="Phi"></param>
        /// <returns></returns>
        static Vector3D GetNormal(double Theta, double Phi)
        {
            return (Vector3D)GetPosition(Theta, Phi, 1.0);
        }

        /// <summary>
        /// Converts 2D Point into 3D Point.
        /// </summary>
        /// <param name="Theta"></param>
        /// <param name="Phi"></param>
        /// <param name="Radius"></param>
        /// <returns></returns>
        internal static Point3D GetPosition(double Theta, double Phi, double Radius)
        {
            double x = (Radius * Math.Sin(Theta)) * Math.Sin(Phi);
            double y = Radius * Math.Cos(Phi);
            return new Point3D(x, y, (Radius * Math.Cos(Theta)) * Math.Sin(Phi));
        }


        #endregion

        #region Rendering



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MeshGeometry3D getMeshGeometry3D()
        {
            //Latitude point Distribution.
            double phiStart = this.GetPhiStart();
            double phiEnd = this.GetPhiEnd();

            int tDiv = 0x10;//16
            int pDiv = 0x10;//16 

            double radius = 1.0;

            //Longitude point Distribution.
            double thetaStart = (((double)this.Column) / ((double)ZoomManager.ZoomLevelColumns(this.ZoomLevel))) * 360.0;
            double thetaEnd = ((this.Column + 1.0) / ((double)ZoomManager.ZoomLevelColumns(this.ZoomLevel))) * 360.0;

            return Tessellate(tDiv, pDiv, radius, thetaStart, phiStart, thetaEnd, phiEnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ModelVisual3D GetModelVisual3D()
        {
            GeometryModel3D modeld = new GeometryModel3D();
            MeshGeometry3D geometryd = this.getMeshGeometry3D();
            modeld.Geometry = geometryd;
            ModelVisual3D visuald = new ModelVisual3D();
            visuald.Content = modeld;
            return visuald;
        }

        /// <summary>
        /// A tessellation or tiling of the plane is a collection of plane figures that fills the plane with 
        /// no overlaps and no gaps. One may also speak of tessellations of the parts of the plane or of other surfaces.
        /// http://en.wikipedia.org/wiki/Tesselate
        /// </summary>
        /// <param name="tDiv"></param>
        /// <param name="pDiv"></param>
        /// <param name="radius"></param>
        /// <param name="ThetaStart"></param>
        /// <param name="PhiStart"></param>
        /// <param name="ThetaEnd"></param>
        /// <param name="PhiEnd"></param>
        /// <returns>MeshGeometry3D</returns>
        internal static MeshGeometry3D Tessellate(int tDiv, int pDiv, double radius, double ThetaStart, double PhiStart, double ThetaEnd, double PhiEnd)
        {
            double slopeX = GISHelper.DegToRad(ThetaEnd - ThetaStart) / ((double)tDiv);
            double slopeY = GISHelper.DegToRad(PhiEnd - PhiStart) / ((double)pDiv);

            // Fill the Position, Normals, and TextureCoordinates collections
            MeshGeometry3D geometryd = new MeshGeometry3D();
            for (int stack = 0; stack <= pDiv; stack++)
            {
                double phi = stack * slopeY;
                phi += GISHelper.DegToRad(PhiStart);
                for (int slice = 0; slice <= tDiv; slice++)
                {
                    double theta = slice * slopeX;
                    theta += GISHelper.DegToRad(ThetaStart);
                    geometryd.Positions.Add(GetPosition(theta, phi, radius));
                    geometryd.Normals.Add(GetNormal(theta, phi));
                }
            }

            //Get Texture co-ordinates.
            geometryd.TextureCoordinates = GetTextureCoordinates(tDiv, pDiv, ThetaStart, PhiStart, ThetaEnd, PhiEnd);

            // Fill the TriangleIndices collection.
            for (int stack = 0; stack < pDiv; stack++)
            {
                for (int slice = 0; slice < tDiv; slice++)
                {
                    int topL = slice;
                    int topR = slice + 1;
                    int bottomL = stack * (tDiv + 1);
                    int bottomR = (stack + 1) * (tDiv + 1);
                    geometryd.TriangleIndices.Add(topL + bottomL);
                    geometryd.TriangleIndices.Add(topL + bottomR);
                    geometryd.TriangleIndices.Add(topR + bottomL);
                    geometryd.TriangleIndices.Add(topR + bottomL);
                    geometryd.TriangleIndices.Add(topL + bottomR);
                    geometryd.TriangleIndices.Add(topR + bottomR);
                }
            }

            geometryd.Freeze();

            return geometryd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tDiv"></param>
        /// <param name="pDiv"></param>
        /// <param name="thetaStart"></param>
        /// <param name="phiStart"></param>
        /// <param name="thetaEnd"></param>
        /// <param name="phiEnd"></param>
        /// <returns></returns>
        static PointCollection GetTextureCoordinates(int tDiv, int pDiv, double thetaStart, double phiStart, double thetaEnd, double phiEnd)
        {
            double radi = (GISHelper.LatitudeToMercatorY(GISHelper.DegToRad(45.0)) * 7.1288559127654789) / 2.0;
            double invRadi = -radi;
            PointCollection points = new PointCollection();
            double elipseX = GISHelper.DegToRad(thetaEnd - thetaStart) / ((double)tDiv);
            double elipseY = GISHelper.DegToRad(phiEnd - phiStart) / ((double)pDiv);
            for (int i = 0; i <= pDiv; i++)
            {
                double times = i * elipseY;
                times += GISHelper.DegToRad(phiStart);
                for (int k = 0; k <= tDiv; k++)
                {
                    double num10 = k * elipseX;
                    num10 += GISHelper.DegToRad(thetaStart);
                    Point point = new Point(num10 / 6.2831853071795862, times / 3.1415926535897931);
                    double phi = times - 1.5707963267948966;
                    point.Y = GISHelper.LatitudeToMercatorY(phi); ;
                    points.Add(point);
                }
            }
            double minValue = double.MinValue;
            double maxValue = double.MaxValue;
            foreach (Point point2 in points)
            {
                if ((point2.Y > minValue) && !double.IsPositiveInfinity(point2.Y))
                {
                    minValue = point2.Y;
                }
                if ((point2.Y < maxValue) && !double.IsNegativeInfinity(point2.Y))
                {
                    maxValue = point2.Y;
                }
            }
            minValue = Math.Min(minValue, radi);
            maxValue = Math.Max(maxValue, invRadi);
            for (int j = 0; j < points.Count; j++)
            {
                Point point6;
                Point point9;
                Point point3 = points[j];
                if (!double.IsPositiveInfinity(point3.Y))
                {
                    Point point4 = points[j];
                    if (point4.Y <= minValue)
                    {
                        point6 = points[j];
                        if (!double.IsNegativeInfinity(point6.Y))
                        {
                            Point point7 = points[j];
                            if (point7.Y >= maxValue)
                            {
                                point9 = PopulatePoints(points, minValue, maxValue, j);
                                continue;
                            }
                        }
                        Point point8 = points[j];
                        points[j] = new Point(point8.X, maxValue);
                    }
                }
                Point point5 = points[j];
                points[j] = new Point(point5.X, minValue);
                point9 = PopulatePoints(points, minValue, maxValue, j);

            }
            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        static Point PopulatePoints(PointCollection points, double minValue, double maxValue, int j)
        {
            Point point9;
            point9 = points[j];
            Point point10 = points[j];
            points[j] = new Point(point9.X, point10.Y - maxValue);
            Point point11 = points[j];
            Point point12 = points[j];
            points[j] = new Point(point11.X, point12.Y / (minValue - maxValue));
            return point9;
        }

        /// <summary>
        /// 
        /// </summary>
        void SetMaterial()
        {
            ImageSource image = (ImageSource)new ImageSourceConverter().ConvertFromString((Globe.BaseLayer as ImageTileLayer).GetImageUrl(Column, Row, this.ZoomLevel));
            ImageBrush brush = new ImageBrush(image);
           // MaterialGroup mg = new MaterialGroup();
            DiffuseMaterial material = new DiffuseMaterial(brush);
            //mg.Children.Add(material);
            ((GeometryModel3D)this.SphereModel.Content).Material = material;
          
        }



        #endregion

    }
}
