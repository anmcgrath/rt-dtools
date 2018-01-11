using RT.Core.DICOM;
using RT.Core.Planning;
using RT.Core.Utilities.RTMath;

namespace DicomPanel.Core.Render
{
    public class BeamRenderer
    {
        private double coll = 0;
        private double couch = 0;
        public void Render(Beam beam, Camera camera, IRenderContext context, Rectd screenRect, LineType lineType)
        {
            var iso = beam.Isocentre.Position;
            RTCoordinateTransform transform = new RTCoordinateTransform();
            transform.CollimatorAngle = beam.CollimatorAngle;
            transform.GantryAngle = beam.GantryStart;
            transform.CouchAngle = beam.CouchAngle;
            //calculate the initial position from the gantry, couch and sad
            var sourcePosn = new Point3d(iso.X, iso.Y - beam.SAD, iso.Z);
            transform.Transform(sourcePosn, iso, sourcePosn);

            var scrnSource = camera.ConvertWorldToScreenCoords(sourcePosn);

            double x1 = beam.XJaw.NegativeJawPosition;
            double x2 = beam.XJaw.PositiveJawPosition;
            double y1 = beam.YJaw.NegativeJawPosition;
            double y2 = beam.YJaw.PositiveJawPosition;

            //Draw to middle of each jaw
            Point3d X1 = new Point3d(iso.X + x1, iso.Y, iso.Z);

            transform.Transform(X1, iso, X1);

            var sX1 = camera.ConvertWorldToScreenCoords(X1);

            context.DrawLine(scrnSource.X, scrnSource.Y, sX1.X, sX1.Y, DicomColors.Yellow);

            //The v coordinates below represent the corners of the jaws projected at isocentre with gantry=couch=coll=0
            //v11 means the corner of the jaws x1 and y1
            //u is the transformed coordinates
            Point3d v11 = new Point3d(iso.X + x1, iso.Y, iso.Z + y1);
            Point3d v12 = new Point3d(iso.X + x1, iso.Y, iso.Z + y2);
            Point3d v21 = new Point3d(iso.X + x2, iso.Y, iso.Z + y1);
            Point3d v22 = new Point3d(iso.X + x2, iso.Y, iso.Z + y2);
            Point3d u11 = new Point3d(), u12 = new Point3d(), u21 = new Point3d(), u22 = new Point3d();
            transform.Transform(v11, iso, u11);
            transform.Transform(v12, iso, u12);
            transform.Transform(v21, iso, u21);
            transform.Transform(v22, iso, u22);
            //We now have four jaw coordinates in the coordinate system.
            //Now interesect the line between the source and each coordinates and draw lines between those
            Point3d w11, w12, w21, w22;
            w11 = camera.Intersect(sourcePosn, u11);
            w12 = camera.Intersect(sourcePosn, u12);
            w21 = camera.Intersect(sourcePosn, u21);
            w22 = camera.Intersect(sourcePosn, u22);

            var s11 = camera.ConvertWorldToScreenCoords(w11);
            var s12 = camera.ConvertWorldToScreenCoords(w12);
            var s21 = camera.ConvertWorldToScreenCoords(w21);
            var s22 = camera.ConvertWorldToScreenCoords(w22);

            context.DrawLine(s11.X, s11.Y, s21.X, s21.Y, DicomColors.Yellow);
            context.DrawLine(s22.X, s22.Y, s21.X, s21.Y, DicomColors.Yellow);
            context.DrawLine(s22.X, s22.Y, s12.X, s12.Y, DicomColors.Yellow);
            context.DrawLine(s11.X, s11.Y, s12.X, s12.Y, DicomColors.Yellow);

            //Draw the line between source and isocentre
            var line = (sourcePosn - iso);

            if (camera.Normal.Dot(line) == 0)
            {
                var scrnIso = camera.ConvertWorldToScreenCoords(iso);
                context.DrawLine(scrnIso.X, scrnIso.Y, scrnSource.X, scrnSource.Y, DicomColors.Yellow);
            }
            else
            {
                //var scrnIso = camera.ConvertWorldToScreenCoords(camera.Intersect(iso, sourcePosn));
                //context.DrawLine(scrnIso.X, scrnIso.Y, scrnSource.X, scrnSource.Y, DicomColors.Yellow);
                //context.DrawEllipse(scrnIso.X, scrnIso.Y, .02, .02, DicomColors.Yellow);
            }

        }

        private Point3d calculateSourcePosition(Beam beam, RTCoordinateTransform transform)
        {
            var sourcePosn = new Point3d(beam.Isocentre.Position.X, beam.Isocentre.Position.Y - beam.SAD, beam.Isocentre.Position.Z);
            var transformedSourcePosn = new Point3d();
            transform.Transform(sourcePosn, beam.Isocentre.Position, transformedSourcePosn);
            return transformedSourcePosn;
        }
    }
}
