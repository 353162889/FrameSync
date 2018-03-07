using System;
using UnityEngine;

namespace Framework
{
	public delegate void SchedulerHandler(float delay);
	public class SchedulerEntity : IPoolable
	{
		public SchedulerHandler handler{ get; private set;}
		public float delay{ get; private set;}
		public int times{get;private set;}
		public SchedulerEntityState state{ get; set;}
		private float curDelay;
		private float startDelay;
		private int curTimes;

		public void Init(SchedulerHandler handler)
		{
			this.handler = handler;
		}

		public void Init(SchedulerHandler handler,float delay,int times)
		{
			this.Init (handler);
			this.curDelay = 0;
			this.startDelay = 0;
			this.curTimes = 0;
			this.delay = delay;
			this.times = times;
			state = SchedulerEntityState.Init;
		}

		/// <summary>
		/// 返回值  是否能接受下次ontick？
		/// </summary>
		/// <param name="dt">Dt.</param>
		public bool OnTick(float dt)
		{
			if (delay <= 0)
			{
				handler (dt);
				curTimes++;
				return CheckCanOnTick ();
			}
			else
			{
				curDelay += dt;
				if (curDelay >= delay)
				{
					handler (curDelay - startDelay);
					curDelay = curDelay - delay;
					startDelay = curDelay;
					curTimes++;
					return CheckCanOnTick ();
				}
				return true;
			}
		}

		private bool CheckCanOnTick()
		{
			//如果次数已超过了，那么返回false
			if (times > 0 && curTimes >= times)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public void Reset()
		{
			this.delay = -1;
			this.handler = null;
			this.times = -1;
			this.curDelay = 0;
			this.curTimes = 0;
			this.startDelay = 0;
			state = SchedulerEntityState.Init;
		}
	}

	public enum SchedulerEntityState
	{
		Init,
		ToAdd,
		ToRemove,
		ToDoAction,
		Error
	}
}

