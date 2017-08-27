using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core
{
    public class DicomPanelGroup
    {
        public bool IsOrthogonalGroup { get; set; }
        public List<DicomPanelModel> Models { get; set; }

        public void AddModel(DicomPanelModel model)
        {
            //model.Group = model;
            Models.Add(model);
        }
    }
}
