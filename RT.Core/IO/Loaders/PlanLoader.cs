using Dicom;
using RT.Core.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Core.Utilities.RTMath;

namespace RT.Core.IO.Loaders
{
    public class PlanLoader
    {
        public void Load(DicomFile[] files, DicomPlanObject plan)
        {
            DicomFile file = files[0];

            Load(file, plan);
        }

        public void Load(DicomFile file, DicomPlanObject plan)
        {
            string modality = file.Dataset.GetSingleValue<string>(DicomTag.Modality);
            if (!modality.Contains("PLAN"))
                throw (new Exception("DICOM file was not RTPLAN"));

            plan.Clear();

            plan.ClassUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SOPClassUID, "");
            plan.InstanceUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, "");
            plan.ReferencedClassUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.ReferencedSOPClassUID, "");
            plan.ReferencedInstanceUID = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.ReferencedSOPInstanceUID, "");

            plan.Name = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.RTPlanName,"");
            plan.Label = file.Dataset.GetSingleValueOrDefault<string>(DicomTag.RTPlanLabel, "");

            if (string.IsNullOrEmpty(plan.Name))
            {
                plan.Name = plan.Label;
            }

            if (file.Dataset.Contains(DicomTag.DoseReferenceSequence))
            {
                var refDoseSQ = file.Dataset.GetSequence(DicomTag.DoseReferenceSequence);
                foreach (var rd in refDoseSQ)
                {
                    plan.RxDose += rd.GetSingleValueOrDefault(DicomTag.TargetPrescriptionDose, 0.0);
                }            
            }

            plan.IsBrachy = file.Dataset.Contains(DicomTag.SourceSequence);
            if (plan.IsBrachy)
            {
                LoadBrachyPlan(file, plan);
            }
            else
            {
                LoadEBRTPlan(file, plan);
            }
        }

        private void LoadBrachyPlan(DicomFile file, DicomPlanObject plan)
        {
            var sourceSQ = file.Dataset.GetSequence(DicomTag.SourceSequence);
            foreach (var v in sourceSQ)
            {
                BrachySource source = new BrachySource();
                source.SourceNumber = v.GetSingleValueOrDefault(DicomTag.SourceNumber, 1);
                source.IsotopeName = v.GetSingleValueOrDefault(DicomTag.SourceIsotopeName, "");
                source.IsotopeHalfLife = v.GetSingleValueOrDefault(DicomTag.SourceIsotopeHalfLife, 1.0);
                source.SourceDiameter = v.GetSingleValueOrDefault(DicomTag.ActiveSourceDiameter, 1.0);
                source.SourceLength = v.GetSingleValueOrDefault(DicomTag.ActiveSourceLength, 1.0);
                source.SourceType = v.GetSingleValueOrDefault(DicomTag.SourceType, "");
                source.RefAirKermaRate = v.GetSingleValueOrDefault(DicomTag.ReferenceAirKermaRate, 1.0);
                DateTime date = v.GetSingleValueOrDefault(DicomTag.SourceStrengthReferenceDate, DateTime.Now);
                DateTime time = v.GetSingleValueOrDefault(DicomTag.SourceStrengthReferenceTime, DateTime.Now);
                source.StrengthDateTime = date.Date.Add(time.TimeOfDay);
                plan.Sources.Add(source.SourceNumber, source);
            }

            var appSQ = file.Dataset.GetSequence(DicomTag.ApplicationSetupSequence);
            var app = appSQ.Items[0];
            plan.TotalRefAirKerma = app.GetSingleValueOrDefault(DicomTag.TotalReferenceAirKerma, 1.0);
            var channelSQ = app.GetSequence(DicomTag.ChannelSequence);
            foreach (var c in channelSQ)
            {
                BrachyChannel channel = new BrachyChannel();
                channel.RefROINumber = c.GetSingleValueOrDefault(DicomTag.ReferencedROINumber, 0);
                channel.ChannelNumber = c.GetSingleValue<int>(DicomTag.ChannelNumber);
                channel.ChannelLength = c.GetSingleValue<double>(DicomTag.ChannelLength);
                channel.ChannelTotalTime = c.GetSingleValue<double>(DicomTag.ChannelTotalTime);
                channel.SourceMoveType = c.GetString(DicomTag.SourceMovementType);
                channel.ApplicatorNumber = c.GetSingleValue<int>(DicomTag.SourceApplicatorNumber);
                channel.ApplicatorID = c.GetString(DicomTag.SourceApplicatorID);
                channel.Name = channel.ApplicatorID;
                channel.ApplicatorType = c.GetString(DicomTag.SourceApplicatorType);
                channel.ApplicatorLength = c.GetSingleValue<double>(DicomTag.SourceApplicatorLength);
                channel.ApplicatorStepSize = c.GetSingleValue<double>(DicomTag.SourceApplicatorStepSize);
                channel.FinalCumulativeTimeWeight = c.GetSingleValue<double>(DicomTag.FinalCumulativeTimeWeight);
                channel.RefSourceNumber = c.GetSingleValue<int>(DicomTag.ReferencedSourceNumber);

                channel.ControlPoints = new List<BrachyControlPoint>();
                var cps = c.GetSequence(DicomTag.BrachyControlPointSequence);
                foreach (var cpItem in cps)
                {
                    BrachyControlPoint cp = new BrachyControlPoint();
                    cp.Index = cpItem.GetSingleValue<int>(DicomTag.ControlPointIndex);
                    cp.RelativePosition = cpItem.GetSingleValue<double>(DicomTag.ControlPointRelativePosition);
                    //cp.Orientation = cpItem.GetValues<double>(DicomTag.ControlPointOrientation);
                    cp.Position3D = cpItem.GetValues<double>(DicomTag.ControlPoint3DPosition);
                    cp.CumulativeTimeWeight = cpItem.GetSingleValue<double>(DicomTag.CumulativeTimeWeight);

                    if (cpItem.Contains(DicomTag.BrachyReferencedDoseReferenceSequence))
                    {
                        var sq = cpItem.GetSequence(DicomTag.BrachyReferencedDoseReferenceSequence);
                        var refDose = sq.Items[0];
                        cp.DoseRefCumulative = refDose.GetSingleValue<double>(DicomTag.CumulativeDoseReferenceCoefficient);
                        cp.DoseRefNumber = refDose.GetSingleValue<int>(DicomTag.ReferencedDoseReferenceNumber);
                    }

                    channel.ControlPoints.Add(cp);
                }

                //Control Points to Dwell Points
                int cpCount = channel.ControlPoints.Count;
                for (int i = 0; i < cpCount; i += 2)
                {
                    BrachyControlPoint cp0 = channel.ControlPoints[i];
                    BrachyControlPoint cp1 = channel.ControlPoints[i + 1];

                    DwellPoint dwp = new DwellPoint();
                    dwp.Index = cp0.Index / 2;
                    dwp.Name = string.Format("DwellPoint{0}", dwp.Index + 1);
                    dwp.RelativePosition = cp0.RelativePosition;
                    dwp.Position3D = new double[3];
                    cp0.Position3D.CopyTo(dwp.Position3D, 0);
                    dwp.TimeWeight = cp1.CumulativeTimeWeight - cp0.CumulativeTimeWeight;
                    //Orientation
                    //DoseRefCumulative
                    //DoseRefNumber
                    channel.DwellPoints.Add(dwp.Index, dwp);
                }

                plan.Channels.Add(channel.ChannelNumber, channel);
            }
        }

        private void LoadEBRTPlan(DicomFile file, DicomPlanObject plan)
        {
            Dictionary<int, double> beamMetersets = getBeamMetersets(file);

            var beam_sequence = file.Dataset.GetSequence(DicomTag.BeamSequence);
            foreach (var child in beam_sequence)
            {
                Beam beam = new Beam();
                beam.TreatmentMachineName = child.GetSingleValueOrDefault(DicomTag.TreatmentMachineName, "");
                beam.SAD = child.GetSingleValueOrDefault(DicomTag.SourceAxisDistance,100.0);
                beam.MU = beamMetersets[child.GetSingleValue<int>(DicomTag.BeamNumber)];
                beam.DoseRateSet = child.GetSingleValueOrDefault(DicomTag.DoseRateSet,600.0);
                beam.Name = child.GetSingleValueOrDefault(DicomTag.BeamName, "");
                double[] oldISOCenter = new double[3];
                decimal ClAngle = 0;
                decimal oldCouchAngle = 0;
                int oldNominalBeamEnergy = 0;

                beam.FinalCumulativeMetersetWeight = child.GetSingleValueOrDefault<double>(DicomTag.FinalCumulativeMetersetWeight, 1.0);

                int numberOfBlocks = child.GetSingleValueOrDefault(DicomTag.NumberOfBlocks, 0);
                if (numberOfBlocks > 0)
                {
                    beam.Blocks = new List<Block>();

                    var block_sequence = child.GetSequence(DicomTag.BlockSequence);

                    foreach (var block_item in block_sequence)
                    {
                        Block block = new Block();
                        block.MaterialID = block_item.GetSingleValueOrDefault(DicomTag.MaterialID, "");
                        block.BlockTrayID = block_item.GetSingleValueOrDefault(DicomTag.BlockTrayID, "");
                        block.SourceToBlockTrayDistance = block_item.GetSingleValueOrDefault(DicomTag.SourceToBlockTrayDistance, 800.0);
                        block.BlockType = block_item.GetSingleValueOrDefault(DicomTag.BlockType, "");
                        block.BlockDivergence = block_item.GetSingleValueOrDefault(DicomTag.BlockDivergence, "");
                        block.BlockNumber = block_item.GetSingleValueOrDefault(DicomTag.BlockNumber, "");
                        block.Name = block_item.GetSingleValueOrDefault(DicomTag.BlockName, "");
                        block.BlockThickness = block_item.GetSingleValueOrDefault(DicomTag.BlockThickness, 15.0);
                        block.Vertices = block_item.GetValues<double>(DicomTag.BlockData);

                        beam.Blocks.Add(block);
                    }
                }

                int numCtrlPoints = child.GetSingleValueOrDefault<int>(DicomTag.NumberOfControlPoints, 0);

                var control_point_sequence = child.GetSequence(DicomTag.ControlPointSequence);

                foreach (var control_point in control_point_sequence)
                {
                    ControlPoint cp = new ControlPoint();

                    cp.Index = control_point.GetSingleValue<int>(DicomTag.ControlPointIndex);

                    if(cp.Index == 0)
                    { 
                        cp.Isocenter = control_point.GetValues<double>(DicomTag.IsocenterPosition);
                        
                        oldISOCenter = cp.Isocenter;
                        cp.CollimatorAngle = control_point.GetSingleValueOrDefault<decimal>(DicomTag.BeamLimitingDeviceAngle, 0);
                        ClAngle = cp.CollimatorAngle;
                        cp.CouchAngle = control_point.GetSingleValueOrDefault<decimal>(DicomTag.PatientSupportAngle, 0);
                        oldCouchAngle = cp.CouchAngle;
                        cp.NominalBeamEnergy = control_point.GetSingleValueOrDefault<int>(DicomTag.NominalBeamEnergy, 0);
                        oldNominalBeamEnergy = cp.NominalBeamEnergy ;
                    }
                    else
                    {
                        cp.Isocenter = oldISOCenter;
                        cp.CollimatorAngle = ClAngle;
                        cp.CouchAngle = oldCouchAngle;
                        cp.NominalBeamEnergy = oldNominalBeamEnergy;
                    }                    

                    cp.CumulativeMetersetWeight = control_point.GetSingleValue<double>(DicomTag.CumulativeMetersetWeight);

                    try
                    {                   
                        if (cp.Index < numCtrlPoints)
                        {                        
                            cp.GantryAngle = control_point.GetSingleValueOrDefault<decimal>(DicomTag.GantryAngle,-1);                 
                        
                            var beam_limiting_sequence = control_point.GetSequence(DicomTag.BeamLimitingDevicePositionSequence);
                            foreach (var beamlimit in beam_limiting_sequence)
                            {
                                string type = beamlimit.GetSingleValueOrDefault(DicomTag.RTBeamLimitingDeviceType, "");
                                var posns = beamlimit.GetValues<double>(DicomTag.LeafJawPositions);

                                if (type == "ASYMX" || type == "X")
                                {
                                    cp.XJaw = new Jaw()
                                    {
                                        Direction = Geometry.Direction.X,
                                        NegativeJawPosition = posns[0],
                                        PositiveJawPosition = posns[1]                                        
                                    };
                                }
                                else if (type == "ASYMY" || type == "Y")
                                {
                                    cp.YJaw = new Jaw()
                                    {
                                        Direction = Geometry.Direction.Y,
                                        NegativeJawPosition = posns[0],
                                        PositiveJawPosition = posns[1]
                                    };
                                }
                                else if (type == "MLCX")
                                {
                                    string tX1 = "|[";
                                    string tX2 = "|[";
                                    int lNum = posns.Length / 2;

                                    for (int xx = 0; xx < lNum; xx++)
                                    {
                                        tX1 += posns[xx] + ",";
                                        tX2 += posns[lNum+xx] + ",";
                                    }

                                    tX1 = tX1.Substring(0, tX1.Length - 1);
                                    tX2 = tX2.Substring(0, tX2.Length - 1);

                                    tX1 += "]|";
                                    tX2 += "]|";

                                    cp.LeafX1 = tX1;
                                    cp.LeafX2 = tX2;
                                }
                            }
                            beam.ControlPoints.Add(cp);

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                if (oldISOCenter.Length == 3)
                {
                    beam.Isocenter = new PointOfInterest(new Point3d((double)oldISOCenter[0], (double)oldISOCenter[1], (double)oldISOCenter[2]));
                }
                else
                {
                    beam.Isocenter = new PointOfInterest(new Point3d(0.0, 0.0,0.0));
                }

                plan.Beams.Add(beam.Name,beam);
            }
        }

        public  Dictionary<int, double> getBeamMetersets(DicomFile file)
        {
            Dictionary<int, double> metersets = new Dictionary<int, double>();
            //only works if one fraction...
            var fraction_group_sequence = file.Dataset.GetSequence(DicomTag.FractionGroupSequence);
            var ref_beam_sequence = fraction_group_sequence.First().GetSequence(DicomTag.ReferencedBeamSequence);
            foreach (var ref_beam in ref_beam_sequence)
            {
                try
                {
                    metersets.Add(ref_beam.GetSingleValueOrDefault<int>(DicomTag.ReferencedBeamNumber, 0), ref_beam.GetSingleValueOrDefault<double>(DicomTag.BeamMeterset, 0.0f));
                }
                catch
                {

                }                
            }
            return metersets;
        }
    }
}
