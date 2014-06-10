namespace WPF.Globe.ClientControl.GIS
{
    public class BBox
    {

        public int x;
        public int y;
        public int width;
        public int height;

        public BBox(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
