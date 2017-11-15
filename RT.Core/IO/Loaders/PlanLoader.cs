using Dicom;
using RT.Core.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RT.Core.IO.Loaders
{
    public class PlanLoader
    {
        public void Load(DicomFile[] files, DicomPlanObject plan)
        {
            DicomFile file = files[0];
            string modality = file.Dataset.Get<string>(DicomTag.Modality);
            if (!modality.Contains("PLAN"))
                throw (new Exception("DICOM file was not RTPLAN"));

            plan.Name = file.Dataset.Get<string>(DicomTag.RTPlanName, "");
            plan.Label = file.Dataset.Get<string>(DicomTag.RTPlanLabel, "");

            Dictionary<int, double> beamMetersets = getBeamMetersets(file);

            var beam_sequence = file.Dataset.Get<DicomSequence>(DicomTag.BeamSequence);
            foreach (var child in beam_sequence)
            {
                Beam beam = new Beam();
                beam.TreatmentMachineName = child.Get(DicomTag.TreatmentMachineName, "");
                beam.SAD = child.Get(DicomTag.SourceAxisDistance, 100.0);
                beam.MU = beamMetersets[child.Get<int>(DicomTag.BeamNumber)];
                beam.Name = child.Get(DicomTag.BeamName, "");


                var totalCumMetersetWeight = child.Get<double>(DicomTag.FinalCumulativeMetersetWeight, 0, 1);

                var control_point_sequence = child.Get<DicomSequence>(DicomTag.ControlPointSequence);
                foreach (var control_point in control_point_sequence)
                {
                    ControlPoint cp = new ControlPoint();
                    cp.Isocentre = child.Get<double[]>(DicomTag.IsocenterPosition, new double[3]);
                    cp.CumulativeMetersetWeight = control_point.Get<double>(DicomTag.CumulativeMetersetWeight);
                    cp.Index = control_point.Get<int>(DicomTag.ControlPointIndex);
                    if (cp.CumulativeMetersetWeight != totalCumMetersetWeight)
                    {
                        cp.NominalBeamEnergy = control_point.Get<int>(DicomTag.NominalBeamEnergy, 0, 0);
                        cp.GantryAngle = control_point.Get(DicomTag.GantryAngle, 0.0);
                        cp.CollimatorAngle = control_point.Get(DicomTag.BeamLimitingDeviceAngle, 0.0);
                        cp.CouchAngle = control_point.Get(DicomTag.PatientSupportAngle, 0.0);


                        var beam_limiting_sequence = control_point.Get<DicomSequence>(DicomTag.BeamLimitingDevicePositionSequence);
                        foreach (var beamlimit in beam_limiting_sequence)
                        {
                            string type = beamlimit.Get(DicomTag.RTBeamLimitingDeviceType, "");
                            var posns = beamlimit.Get<double[]>(DicomTag.LeafJawPositions);

                            if (type == "ASYMX" || type == "X")
                            {
                                cp.XJaw = new Jaw()
                                {
                                    Direction = Geometry.Direction.X,
                                    NegativeJawPosition = posns[0],
                                    PositiveJawPosition = posns[1],
                                };
                            }
                            else if (type == "ASYMY" || type == "Y")
                            {
                                cp.YJaw = new Jaw()
                                {
                                    Direction = Geometry.Direction.Y,
                                    NegativeJawPosition = posns[0],
                                    PositiveJawPosition = posns[1],
                                };
                            }
                            else if (type == "MLCX")
                            {

                            }
                        }
                    }
                    beam.ControlPoints.Add(cp);
                }
                plan.Beams.Add(beam);
            }
        }

        public  Dictionary<int, double> getBeamMetersets(DicomFile file)
        {
            Dictionary<int, double> metersets = new Dictionary<int, double>();
            //only works if one fraction...
            var fraction_group_sequence = file.Dataset.Get<DicomSequence>(DicomTag.FractionGroupSequence);
            var ref_beam_sequence = fraction_group_sequence.First().Get<DicomSequence>(DicomTag.ReferencedBeamSequence);
            foreach (var ref_beam in ref_beam_sequence)
            {
                metersets.Add(ref_beam.Get<int>(DicomTag.ReferencedBeamNumber), ref_beam.Get<double>(DicomTag.BeamMeterset));
            }
            return metersets;
        }
    }
}
