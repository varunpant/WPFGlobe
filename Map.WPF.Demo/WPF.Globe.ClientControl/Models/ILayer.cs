namespace WPF.Globe.ClientControl.Models
{
    public interface ILayer
    {
        /// <summary>
        /// A unique ID to idenitify the layer
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Name of this Layer.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewMode"></param>
        void SwitchMapMode(string NewMode);

        /// <summary>
        /// Gets or sets the current map mode.
        /// </summary>
        string CurrentMapMode { get;  }
    }
}
