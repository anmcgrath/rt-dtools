using RT.Core.ROIs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RTDicomViewer.Converters
{
    public class ContourTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                var type = (ContourType)value;
                switch(type)
                {
                    case ContourType.POINT:
                        return @"\RTDicomViewer;component/Resources/poi.png";
                    case ContourType.CLOSED_PLANAR:
                        return @"\RTDicomViewer;component/Resources/contour_closed.png";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
