using Li3.MiningOutpost.Models;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Voxels;
using VRageMath;

namespace Li3.MiningOutpost.Components
{
	public class DirectionalOreFinderComponent : MyEntityComponentBase
	{
		const int NoVoxelCollisionLayer = 9;
		const int VoxelLod1CollisionLayer = 11;

		public bool HasUnobstructedLineOfSight => IsLineOfSightClear();
		public List<VoxelDefinitionAndBuilder> OresDetected { get; private set; }

		private Vector3D scanDirectionField;
		public Vector3D ScanDirection
		{
			get { return scanDirectionField; }
			set { scanDirectionField = Vector3D.Normalize(value); }
		}

		public float Distance { get; set; }
		public bool ShouldDoWork { get; set; }

		public DirectionalOreFinderComponent()
		{
			OresDetected = new List<VoxelDefinitionAndBuilder>();
		}

		private bool IsLineOfSightClear()
		{
			var non_voxel_raycasts = RaycastAllButVoxels();
			if (non_voxel_raycasts.Count > 0)
			{
				return false;
			}

			return true;
		}

		public void Update()
		{
			OresDetected.Clear();

			if (ShouldDoWork && IsLineOfSightClear())
			{
				var voxel_raycasts = RaycastVoxels();
				var voxels = MapRaycastsToVoxels(voxel_raycasts);
				var ores = MapVoxelsToOre(voxels);

				UpdateOresDetected(ores);
			}
		}

		private void UpdateOresDetected(List<MyVoxelMaterialDefinition> ores)
		{
			ores.ForEach(ore =>
			{
				if (!ore.CanBeHarvested)
					return;

				var item = new VoxelDefinitionAndBuilder()
				{
					Definition = ore,
					Builder = MyObjectBuilderSerializer.CreateNewObject<MyObjectBuilder_Ore>(ore.MinedOre)
				};

				OresDetected.Add(item);
			});
		}

		private List<VoxelEntityAtPosition> MapRaycastsToVoxels(List<IHitInfo> raycasts)
		{
			var voxels = new List<VoxelEntityAtPosition>();
			raycasts.ForEach(hitinfo =>
			{
				if (hitinfo.HitEntity is IMyVoxelBase)
				{
					voxels.Add(new VoxelEntityAtPosition(hitinfo.HitEntity as IMyVoxelBase, hitinfo.Position));
				}
			});

			return voxels;
		}

		private List<MyVoxelMaterialDefinition> MapVoxelsToOre(List<VoxelEntityAtPosition> voxels)
		{
			var materials = new List<MyVoxelMaterialDefinition>();
			voxels.ForEach(voxel =>
			{
				var material = GetOreAtPosition(voxel.Voxel, voxel.Position);
				if (material != null && !materials.Contains(material))
				{
					materials.Add(material);
				}
			});

			return materials;
		}

		private MyVoxelMaterialDefinition GetOreAtPosition(IMyVoxelBase voxel, Vector3D position)
		{
			if (voxel == null || voxel.Storage == null)
				return null;

			var storage = new MyStorageData();

			Vector3I voxelMin, voxelMax;
			MyVoxelCoordSystems.WorldPositionToVoxelCoord(voxel.PositionLeftBottomCorner, ref position, out voxelMin);
			MyVoxelCoordSystems.WorldPositionToVoxelCoord(voxel.PositionLeftBottomCorner, ref position, out voxelMax);

			var voxelBase = voxel as MyVoxelBase;
			voxelMin += voxelBase.StorageMin;
			voxelMax += voxelBase.StorageMin;

			voxel.Storage.ClampVoxel(ref voxelMin);
			voxel.Storage.ClampVoxel(ref voxelMax);

			storage.Resize(voxelMin, voxelMax);
			voxel.Storage.ReadRange(storage, MyStorageDataTypeFlags.ContentAndMaterial, 0, voxelMin, voxelMax);

			return MyDefinitionManager.Static.GetVoxelMaterialDefinition(storage.Material(0));
		}

		private Vector3D GetTarget()
		{
			var offset = scanDirectionField * Distance;
			return Vector3D.Transform(offset, Entity.WorldMatrix);
		}

		private List<IHitInfo> RaycastVoxels()
		{
			var hitinfos = new List<IHitInfo>();

			MyAPIGateway.Physics.CastRay(Entity.PositionComp.GetPosition(), GetTarget(), hitinfos, VoxelLod1CollisionLayer);

			return hitinfos;
		}

		private List<IHitInfo> RaycastAllButVoxels()
		{
			var hitinfos = new List<IHitInfo>();

			MyAPIGateway.Physics.CastRay(Entity.PositionComp.GetPosition(), GetTarget(), hitinfos, NoVoxelCollisionLayer);

			return hitinfos;
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
