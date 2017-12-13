using DicomPanel.Core.Render;
using GalaSoft.MvvmLight;
using RT.Core.Dose;
using RT.Core.Imaging.LUT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    /// <summary>
    /// Wrapper for dose grid for the dose object display view model
    /// </summary>
    public class DoseGridWrapper:ViewModelBase
    {
        public IDoseObject Dose { get; set; }
        /// <summary>
        /// Whether or not to render contour lines
        /// </summary>
        public bool RenderLines { get { return _renderLines; } set { Set("RenderLines", ref _renderLines, value); } }
        private bool _renderLines;
        /// <summary>
        /// Whether or not to render dose wash
        /// </summary>
        public bool RenderDoseWash { get { return _renderDoseWash; } set { Set("RenderDoseWash", ref _renderDoseWash, value); } }
        private bool _renderDoseWash;
        /// <summary>
        /// 
        /// </summary>
        public LUTType LUTType { get { return _lutType; } set { Set("LUTType", ref _lutType, value); } }
        private LUTType _lutType;

        /// <summary>
        /// The LUT for the dose wash
        /// </summary>
        public ILUT LUT { get; set; }

        public RenderableImage RenderableImage { get; set; }

        public int WindowBottom { get; set; }
        public float WindowTop { get; set; }
    }
}
