using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Event
{
    public class CameraTranslatedEventArgs
    {
        TranslationType Type { get; set; }
        public CameraTranslatedEventArgs(TranslationType type)
        {
            Type = type;
        }
    }
}
