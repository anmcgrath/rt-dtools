using DicomPanel.Core.Render;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using RT.Core.DICOM;

namespace RTDicomViewer.Converters
{
    public class DicomColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DicomColor)
            {
                var dicomColor = (DicomColor)value;
                return Color.FromArgb((byte)dicomColor.A, (byte)dicomColor.R, (byte)dicomColor.G, (byte)dicomColor.B);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                var color = (Color)value;
                return DicomColor.FromArgb(color.A, color.R, color.G, color.B);
            }
            return null;
        }
    }
}
