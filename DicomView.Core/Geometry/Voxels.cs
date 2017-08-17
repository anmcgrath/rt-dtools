using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomPanel.Core.Geometry
{
    public class Voxels : IEnumerable<Voxel>
    {
        private IVoxelDataStructure _voxelDataStructure;
        public Voxels(IVoxelDataStructure voxelDataStructure)
        {
            _voxelDataStructure = voxelDataStructure;
        }

        IEnumerator<Voxel> IEnumerable<Voxel>.GetEnumerator()
        {
            return _voxelDataStructure;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _voxelDataStructure;
        }
    }
}
