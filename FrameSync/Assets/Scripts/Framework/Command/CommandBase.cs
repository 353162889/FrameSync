using System;

namespace Framework
{
	public enum CmdExecuteState
	{
		Success,
		Fail
	}

    public enum CmdRunState
    {
        Init,
        Runing,
        Done
    }

	public class CommandBase
	{
		public event Action<CommandBase> On_Done;

		public CommandContainerBase Parent{get;set;}

		public CmdExecuteState State{ get; protected set;}

        public CmdRunState RunState { get; private set; }

		protected ICommandContext _context;
        public ICommandContext Context {
            get { return _context; }
        }

        public CommandBase()
        {
            this.RunState = CmdRunState.Init;
        }

		public void Execute()
		{
			this.Execute (null);
		}

		public virtual void Execute(ICommandContext context)
		{
			this.State = CmdExecuteState.Success;
            this.RunState = CmdRunState.Runing;
			this._context = context;
		}

		protected virtual void OnExecuteDone(CmdExecuteState state)
		{
			this.State = state;
			OnExecuteFinish ();
			OnDoneInvoke ();

			if (Parent != null)
			{
				Parent.OnChildDone (this);
			}
			else
			{
				this.OnDestroy ();
			}
		}
		/// <summary>
		/// 自己执行完成，在告诉父对象之前
		/// </summary>
		protected virtual void OnExecuteFinish()
		{
            this.RunState = CmdRunState.Done;
		}
		/// <summary>
		/// 回调执行完成的监听
		/// </summary>
		protected void OnDoneInvoke()
		{
			if (On_Done != null)
			{
				On_Done.Invoke (this);
			}
		}

        /// <summary>
        /// 用于内部更新处理，如果需要，外部调用
        /// </summary>
        public virtual void OnUpdate()
        {

        }

		public virtual void OnDestroy()
		{
            if(this.Parent == null)
            {
                if (this._context != null) this._context.Dispose();
                this._context = null;
            }
            this.Parent = null;
			On_Done = null;
		}

		public virtual void Reset()
		{
            if (this.Parent == null)
            {
                if (this._context != null) this._context.Dispose();
                this._context = null;
            }
            this.RunState = CmdRunState.Init;
        }
	}
}

