using System;
using System.Collections.Generic;
using VRage.Game.Components;
using VRage.ModAPI;

namespace Li3.MiningOutpost.Components
{
	public class ConiditonalComponent : MyEntityComponentBase
	{
		public List<Func<IMyEntity, bool>> Conditions { get; private set; }

		private bool previousState;
		public string ConditionLabel { get; set; }

		public List<Action<bool, bool>> Subscribers { get; private set; }

		public ConiditonalComponent()
		{
			Conditions = new List<Func<IMyEntity, bool>>();
			Subscribers = new List<Action<bool, bool>>();
		}

		public void Init(IMyEntity entity, string label)
		{
			ConditionLabel = label;
		}

		public void Update()
		{
			TestConditions();
		}

		private bool TestConditions()
		{
			var nextState = Conditions.TrueForAll(o => o(Entity));
			if (nextState != previousState)
			{
				Subscribers.ForEach(a => a(previousState, nextState));
			}

			previousState = nextState;

			return nextState;
		}

		public override string ComponentTypeDebugString => string.Format("{0}.{1}", Entity.DisplayName, GetType().Name);
	}
}
