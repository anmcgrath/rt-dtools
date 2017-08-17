using DicomPanel.Core.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DicomPanel.Wpf.Rendering
{
    public class DicomColorConverter
    {
        public static Color FromDicomColor(DicomColor color)
        {
            return Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
        }
    }
}
