using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class Scheduler : MonoBehaviour
	{
		private bool _isUpdating;
		//这个列表里的SchedulerEntityState状态只代表是否执行该Entity，不代表移除（或添加）
		private List<SchedulerEntity> _listSchedulerEntity;
		//这个列表里的SchedulerEntityState状态只代表是否移除（添加）Entity
		private List<SchedulerEntity> _listOperateEntity;
		private ObjectPool<SchedulerEntity> _pool;

        protected virtual void Awake()
        {
            Init();
        }
        protected void Init ()
		{
			_pool = ObjectPool<SchedulerEntity>.Instance;
			_pool.Init (10);
			_listSchedulerEntity = new List<SchedulerEntity> ();
			_listOperateEntity = new List<SchedulerEntity> ();
			_isUpdating = false;

		}

		public bool AddScheduler(SchedulerHandler handler,float delay,int times = 0)
		{
			if (HasScheduler (handler))
			{
				return false;
			}
			SchedulerEntity entity = _pool.GetObject ();
			entity.Init (handler,delay, times);
			if (_isUpdating)
			{
				entity.state = SchedulerEntityState.ToAdd;
				_listOperateEntity.Add (entity);
			}
			else
			{
				entity.state = SchedulerEntityState.ToDoAction;
				_listSchedulerEntity.Add (entity);
			}
			return true;
		}

		public void RemoveScheduler(SchedulerHandler handler)
		{
			if (_isUpdating)
			{
				SchedulerEntity entity = _pool.GetObject ();
				entity.Init (handler);
				entity.state = SchedulerEntityState.ToRemove;
				_listOperateEntity.Add (entity);
				//有可能当前的scheduleEntity.handler里移除了一个即将调用的scheduleEntity（被移掉的不再执行）
				for (int i = 0; i < _listSchedulerEntity.Count; i++)
				{
					if (_listSchedulerEntity [i].handler == handler)
					{
						_listSchedulerEntity [i].state = SchedulerEntityState.ToRemove;
					}
				}
			}
			else
			{
				RealRemoveSchedule (handler);
			}
		}

		private void RealRemoveSchedule(SchedulerHandler handler)
		{
			for (int i = _listSchedulerEntity.Count - 1; i >= 0; i--)
			{
				if (_listSchedulerEntity [i].handler == handler)
				{
					SchedulerEntity entity = _listSchedulerEntity [i];
					_listSchedulerEntity.RemoveAt (i);
					_pool.SaveObject (entity);
					break;
				}
			}
		}

		public bool HasScheduler(SchedulerHandler handler)
		{
			bool result = false;
			for (int i = 0; i < _listSchedulerEntity.Count; i++)
			{
				if (_listSchedulerEntity [i].handler == handler)
				{
					result = true;
					break;
				}
			}
			if (_isUpdating)
			{
				int count = result ? 1 : 0;
				for (int i = 0; i < _listOperateEntity.Count; i++)
				{
					if (_listOperateEntity [i].handler == handler)
					{
						if (_listOperateEntity [i].state == SchedulerEntityState.ToAdd)
						{
							count++;
						}
						else if (_listOperateEntity [i].state == SchedulerEntityState.ToRemove)
						{
							count--;
						}
					}
				}
				result = count > 0;
			}
			return result;
		}

		protected void OnTick(float dt)
		{
			_isUpdating = true;
//			CLog.Log ("_listSchedulerEntity.count:"+_listSchedulerEntity.Count);
			for (int i = 0; i < _listSchedulerEntity.Count; i++)
			{
				SchedulerEntity entity = _listSchedulerEntity [i];
				if (entity.state == SchedulerEntityState.Error)
					continue;
				if (entity.state == SchedulerEntityState.ToDoAction)
				{
					try
					{
						if (!entity.OnTick (dt))
						{
							entity.state = SchedulerEntityState.ToRemove;

							SchedulerEntity RemoveEntity = _pool.GetObject ();
							RemoveEntity.Init (entity.handler);
							RemoveEntity.state = SchedulerEntityState.ToRemove;
							_listOperateEntity.Add (RemoveEntity);
						}
					}
					//捕获异常，防止有异常后整个scheduler全部卡死
					catch(System.Exception ex)
					{
						entity.state = SchedulerEntityState.Error;
						CLog.LogError (ex.Message+"\n"+ex.StackTrace);	
					}
				}
			}
			_isUpdating = false;
			for (int i = 0; i < _listOperateEntity.Count; i++)
			{
				SchedulerEntity entity = _listOperateEntity [i];
				if (entity.state == SchedulerEntityState.ToAdd)
				{
                    AddScheduler(entity.handler, entity.delay, entity.times);
				}
				else if (entity.state == SchedulerEntityState.ToRemove)
				{
                    RemoveScheduler(entity.handler);
				}
                _pool.SaveObject(entity);
			}
			_listOperateEntity.Clear ();
		}

        protected void OnDestroy ()
		{
			_listOperateEntity.Clear ();
			_listSchedulerEntity.Clear ();
		}
	}
}

