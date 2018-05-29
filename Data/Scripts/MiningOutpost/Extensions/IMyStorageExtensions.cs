using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Li3.MiningOutpost
{
    public static class IMyStorageExtensions
    {
        public static void ClampVoxel(this VRage.ModAPI.IMyStorage self, ref Vector3I voxelCoord, int distance = 1)
        {
            if (self == null) return;
            var sizeMinusOne = self.Size - distance;
            Vector3I.Clamp(ref voxelCoord, ref Vector3I.Zero, ref sizeMinusOne, out voxelCoord);
        }
    }
}
