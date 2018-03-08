using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 顺序执行器，失败停止
    /// </summary>
	public class CommandSequence : CommandContainerBase
	{
		protected LinkedList<CommandBase> _children;
		protected CommandBase _executeChild;
        protected bool _isChildFailStop;
		public bool IsExecuting{get{ return _executeChild != null; }}
        public int ChildCount {
            get { return _children.Count; }
        }

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="childFailStop">是否子命令执行失败时停止？（停止后的状态是失败，否则是成功）</param>
		public CommandSequence(bool childFailStop = true):base()
		{
            this._isChildFailStop = childFailStop;
			_children = new LinkedList<CommandBase> ();
		}

		/// <summary>
		/// 添加子命令，调用Execute执行当前添加的子命令
		/// </summary>
		/// <param name="command">Command.</param>
		public override void AddSubCommand(CommandBase command)
		{
			command.Parent = this;
			_children.AddLast (command);
		}

		public override void Execute (ICommandContext context)
		{
			base.Execute (context);
			Next ();
		}
		/// <summary>
		/// 跳过当前正在执行的子对象
		/// </summary>
		public virtual void SkipExecuteChild()
		{
			if (IsExecuting)
			{
				OnChildDestroy (_executeChild);
				_executeChild = null;
			}
			Execute (this._context);
		}

        public override void OnUpdate()
        {
            if(RunState == CmdRunState.Runing && IsExecuting)
            {
                _executeChild.OnUpdate();
            }
        }

        public override void OnChildDone (CommandBase command)
		{
			this.On_ChildDoneCallback (command);
            if(_isChildFailStop)
            { 
			    this.State = command.State;
            }
            OnChildDestroy (command);
            _executeChild = null;
            if (this.State == CmdExecuteState.Fail && _isChildFailStop)
            {
                this.OnExecuteDone(CmdExecuteState.Fail);
            }
            else
            {
                Next();
            }
		}

		protected virtual void Next()
		{
			if (_children.Count > 0)
			{
				_executeChild = _children.First.Value;
				_children.RemoveFirst ();
                this.OnChildStart(_executeChild);
                _executeChild.Execute (this._context);
			}
			else
			{
				this.OnExecuteDone (CmdExecuteState.Success);
			}
		}

		public override void Clear ()
		{
			if (IsExecuting)
			{
				OnChildDestroy (_executeChild);
				_executeChild = null;
			}
			CommandBase child;
			while (_children != null && _children.Count > 0 &&(child = _children.First.Value) != null)
			{
				_children.RemoveFirst ();
				OnChildDestroy (child);
			}
		}

		public override void OnDestroy ()
		{
			Clear ();
			base.OnDestroy ();
		}
	}
}

