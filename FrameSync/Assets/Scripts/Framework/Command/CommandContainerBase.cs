using System;

namespace Framework
{
	public abstract class CommandContainerBase : CommandBase
	{
		public event Action<CommandBase> On_ChildDone;
        public event Action<CommandBase> On_ChildExecute;

        public CommandContainerBase():base()
        {

        }
        public virtual void AddSubCommand(CommandBase command)
        {

        }

        public virtual void OnChildStart(CommandBase command)
        {
            On_ChildExecuteCallback(command);
        }

        /// <summary>
        /// 当前子对象执行完成
        /// </summary>
        /// <param name="command">Command.</param>
        public virtual void OnChildDone(CommandBase command)
		{
			On_ChildDoneCallback (command);
			OnChildDestroy (command);
		}

		/// <summary>
		/// 当前子对象执行完成的回调
		/// </summary>
		/// <param name="command">Command.</param>
		protected void On_ChildDoneCallback(CommandBase command)
		{
			if (On_ChildDone != null)
			{
				this.On_ChildDone (command);
			}
		}

        protected void On_ChildExecuteCallback(CommandBase command)
        {
            if(On_ChildExecute != null)
            {
                this.On_ChildExecute(command);
            }
        }

		/// <summary>
		/// 子对象销毁时（常用于重用子对象）
		/// </summary>
		/// <param name="command">Command.</param>
		protected virtual void OnChildDestroy(CommandBase command)
		{
			command.OnDestroy ();
		}

		/// <summary>
		/// 清除当前所有子对象
		/// </summary>
		public virtual void Clear()
		{

		}

        public virtual bool Cancel()
        {
            return true;
        }

		/// <summary>
		/// 当前对象销毁
		/// </summary>
		public override void OnDestroy ()
		{
			this.On_ChildDone = null;
			base.OnDestroy ();
		}
	}
}

