using VRage.Game.Components;
using VRage.ModAPI;
using VRageMath;

namespace Li3.MiningOutpost.Components
{
	public class WorkingLightsComponent : MyEntityComponentBase
	{
		public enum LightState
		{
			On,
			Off
		};

		private LightState stateField;
		public LightState State
		{
			get { return stateField; }
			set
			{
				SetState(value);
			}
		}
		public Color OnColor { get; set; }
		public Color OffColor { get; set; }
		public float Brightness { get; set; }
		public string[] EmissiveParts { get; set; }

		public WorkingLightsComponent()
		{
			OnColor = Color.Green;
			OffColor = Color.Red;
			Brightness = 1.0f;
		}

		public void Init(IMyEntity entity, string[] emissiveParts)
		{
			EmissiveParts = emissiveParts;
		}

		public void SetState(LightState value)
		{
			stateField = value;
			UpdateLights();
		}

		private void UpdateLights()
		{
			foreach (var part in EmissiveParts)
			{
				Entity.SetEmissiveParts(part, GetColor(), Brightness);
			}
		}

		private Color GetColor()
		{
			return (stateField == LightState.On) ? OnColor : OffColor;
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
