// (C) Copyright 2014 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

//- Written by Cyrille Fauvel, Autodesk Developer Network (ADN)
//- http://www.autodesk.com/joinadn
//- December 30th, 2013
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Autodesk.Maya {

	[ValueConversion (typeof (bool), typeof (double))]
	public class BooleanToDoubleConverter : IValueConverter {

		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			if ( value != null && value is bool ) {
				bool val =System.Convert.ToBoolean (value) ;
				return (val ? 1.0 : 0.0) ;
			}
			return (null) ;
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
			if ( value != null && value is double ) {
				var val =(double)value ;
				return (val == 0) ;
			}
			return (null) ;
		}
	}

	[ValueConversion (typeof (double), typeof (string))]
	public class ProgressBarValueConverter : IValueConverter {

		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			if ( value != null && value is double ) {
				double val =System.Convert.ToDouble (value)  ;
				string strValue =val.ToString ("N0") ;
				return (strValue) ;
			}
			return (value) ;
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
			return (value) ;
		}

	}

	public class ProgressBarPercentageConverter : IMultiValueConverter {

		public object Convert (object [] values, Type targetType, object parameter, CultureInfo culture) {
			double value =System.Convert.ToDouble (values [0]) ;
			double minValue =System.Convert.ToDouble (values [1]) ;
			double maxValue =System.Convert.ToDouble (values [2]) ;
			if ( minValue == maxValue )
				return ("~%") ;
			double val =100 * (value - minValue) / (maxValue - minValue) ;
			string strValue =val.ToString ("N0") + "%" ;
			return (strValue) ;
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException ();
		}

	}

	public class PositionConverter : IMultiValueConverter {

		public object Convert (object [] values, Type targetType, object parameter, CultureInfo culture) {
			double x =System.Convert.ToDouble (values [0]) ;
			double size =System.Convert.ToDouble (values [1]) ;
			return (x - size) ;
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException () ;
		}

	}

	public class Rect2Converter : IMultiValueConverter {

		public object Convert (object [] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double width =System.Convert.ToDouble (values [0]) ;
			double height =System.Convert.ToDouble (values [1]) ;
			return (new Rect (0, 0, width, height)) ;
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException () ;
		}

	}

	public class RectConverter : IMultiValueConverter {

		public object Convert (object [] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double x = System.Convert.ToDouble (values [0]);
			double y = System.Convert.ToDouble (values [1]);
			double width = System.Convert.ToDouble (values [2]);
			double height = System.Convert.ToDouble (values [3]);
			return (new Rect (x, y, width, height));
		}

		public object [] ConvertBack (object value, Type [] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException ();
		}

	}

}
