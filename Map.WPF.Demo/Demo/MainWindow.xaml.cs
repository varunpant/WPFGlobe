using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Globe.ClientControl;
using WPF.Globe.ClientControl.LayerTypes;
using WPF.Globe.ClientControl.Models;
using System.Linq;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ILayer> layers = new List<ILayer>();
       

        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;
            globe1.ZoomLevelChanged += new Globe.ZoomLevelChangedHandler(globe1_ZoomLevelChanged);

            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.BingMapLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                Name = "Bing Layer",
                MapMode = WPF.Globe.ClientControl.LayerTypes.Image_Layers.BingMapLayer.ImageMode.h
            });

            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.YahooMapLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                MapMode = WPF.Globe.ClientControl.LayerTypes.Image_Layers.YahooMapLayer.YhooMapModes.YahooAerial,
                Name = "Yahoo Map Layer"
            });

            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.GoogleEarthMapLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                MapMode = WPF.Globe.ClientControl.LayerTypes.Image_Layers.GoogleEarthMapLayer.GoogleMapModes.Satellite,
                Name = "Google Earth Map Layer"
            });

            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.WmsTileLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                Name = "Wms Tile Layer",
                MapMode=WPF.Globe.ClientControl.LayerTypes.Image_Layers.WmsTileLayer.WMSPaths.NASAGlobalMosaic
            });

            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.GooglePlanetsLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                Name = "Google Planets Layer",
                MapMode = WPF.Globe.ClientControl.LayerTypes.Image_Layers.GooglePlanetsLayer.GpMapModes.GoogleMarsVisible
            });



            layers.Add(new WPF.Globe.ClientControl.LayerTypes.Image_Layers.MapBoxLayer()
            {
                ID = new Guid().ToString().Replace("-", ""),
                Name = "MapBox Layer",
                MapMode = WPF.Globe.ClientControl.LayerTypes.Image_Layers.MapBoxLayer.MapBoxModes.NaturalEarth
            });

            globe1.Initialise(layers[0]);
            ddlLayers.ItemsSource = layers.Select(l => l.Name);
            BindModesCombo("Bing Layer");
          

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="newLevel"></param>
        void globe1_ZoomLevelChanged(object Sender, int newLevel)
        {
            ZoomSlider.Value = newLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.OemPlus) && ((globe1.CurrentZoomLevel <= 11)))
            {
                //  globe1.ZoomGlobe(globe1.CurrentZoomLevel + 1);
                ZoomSlider.Value = globe1.CurrentZoomLevel + 1;
            }
            else if ((e.Key == Key.OemMinus) && ((globe1.CurrentZoomLevel >= 0)))
            {
                //  globe1.ZoomGlobe(globe1.CurrentZoomLevel - 1);
                ZoomSlider.Value = globe1.CurrentZoomLevel - 1;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            double lat = double.Parse(LatitudeTxt.Text);
            double lon = double.Parse(LongitudeTxt.Text);

            globe1.GoToGlobePosition(lat, lon);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            globe1.ZoomGlobe(Convert.ToInt32(e.NewValue));
        }

         

        /// <summary>
        /// 
        /// </summary>
        void BindModesCombo(string currentLayerName)
        {
            ddlModes.Items.Clear();

            ImageTileLayer currentLayer = layers.Where(l=>l.Name == currentLayerName).Single() as ImageTileLayer;

            foreach (var name in currentLayer.MapModes)
            {
                ddlModes.Items.Add(new ComboBoxItem() { Content = name });
            }
            globe1.SwitchBaseLayer(currentLayer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlModes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlModes.SelectedItem != null)
            {
                var currentSelection = (ddlModes.SelectedItem as ComboBoxItem).Content.ToString();
               
                globe1.SwitchMapMode(currentSelection);
            }
        }

        private void ddlLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelection = (ddlLayers.SelectedItem as string);
            BindModesCombo(currentSelection);
        }
    }
}
