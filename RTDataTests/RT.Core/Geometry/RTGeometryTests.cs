using Microsoft.VisualStudio.TestTools.UnitTesting;
using RT.Core.Geometry;

namespace RTDataTests.RT.Core.Geometry
{
    [TestClass]
    public class RTGeometryTests
    {
        public object GridBasedDataStructure { get; private set; }

        [TestMethod]
        public void GridBasedStrctureInterpolation()
        {
            var grid = GridBasedVoxelDataStructure.CreateNew(10, 10, 10, new Range(0, 10), new Range(0, 10), new Range(0, 10));
            grid.ConstantGridSpacing = true;
            grid.SetVoxelByCoords(1, 1, 1, 10);
            Assert.AreEqual(grid.Interpolate(1, 1, 1).Value, 10);
        }
    }
}
