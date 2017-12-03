using RT.Core.Geometry;
using RT.Core.Utilities.RTMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace DicomPanel.Core.Render.Overlays
{
    public class PointInfoOverlay:IOverlay
    {
        public Point3d Position { get; set; }
        private POIOverlay poiOverlay = new POIOverlay();
        private TextOverlay textOverlay = new TextOverlay("");
        public PointInfoOverlay()
        {
            Position = new Point3d();
            poiOverlay = new POIOverlay();
            poiOverlay.KeepSameSizeOnScreen = true;
            poiOverlay.RenderCircle = false;
        }

        public void Render(DicomPanelModel model, IRenderContext context)
        {
            poiOverlay.Position = Position;
            Position.CopyTo(textOverlay.Position);
            textOverlay.Position = Position - (model.Camera.RowDir * 25 / model.Camera.Scale);

            List<string> overlayStrings = new List<string>();
            if (model?.PrimaryImage != null)
                overlayStrings.Add($"Primary Image: {Math.Round((float)(model?.PrimaryImage.Grid.Interpolate(Position).Value*model?.PrimaryImage.Grid.Scaling),2)} {model?.PrimaryImage.Grid.ValueUnit}");
            if (model?.SecondaryImage != null)
                overlayStrings.Add($"Primary Image: {Math.Round((float)(model?.SecondaryImage.Grid.Interpolate(Position).Value*model?.PrimaryImage.Grid.Scaling),2)} {model?.SecondaryImage.Grid.ValueUnit}");
            foreach (var img in model?.AdditionalImages)
                overlayStrings.Add($"{img.Name}: {Math.Round(img.Grid.Interpolate(Position).Value*img.Grid.Scaling,2)} {img.Grid.ValueUnit}");

            int i = 0;

            foreach(var str in overlayStrings)
            {
                textOverlay.Text = str;
                textOverlay.Position = Position - i*(model.Camera.RowDir * 20 / model.Camera.Scale);
                textOverlay.Render(model, context);
                i++;
            }
            

            //textOverlay.Text = "";

            /*if (HU != null)
                textOverlay.Text = Math.Round(HU.Value) + " HU";
            if (doseVoxel != null)
            {
                textOverlay.Text += "\n" + Math.Round(100*(doseVoxel.Value / (double)doseNorm), 2) + "%";
            }*/

            textOverlay.Text = "("+Math.Round(Position.X,2)+", "+Math.Round(Position.Y,2)+", "+Math.Round(Position.Z,2)+") (mm)";
            textOverlay.Position -= (model.Camera.RowDir * 20 / model.Camera.Scale);
            textOverlay.Render(model, context);

            //poiOverlay.Render(model, context);
            //Use the poi fractin of oriignal size to set opacity of text
            //textOverlay.Opacity = poiOverlay.Fraction;


        }
    }
}
