using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.ModAPI;
using VRageMath;

namespace Li3.MiningOutpost
{
	class VoxelEntityAtPosition
	{
		public IMyVoxelBase Voxel;
		public Vector3D Position;

		public VoxelEntityAtPosition(IMyVoxelBase voxel, Vector3D position)
		{
			Voxel = voxel;
			Position = position;
		}
	}
}
