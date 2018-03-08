using System;
using System.Collections.Generic;

namespace Framework
{
	public class CommandImmediately : CommandContainerBase
	{
		protected LinkedList<CommandBase> _children;
		protected bool _isChildFailStop;
		public bool IsExecuting{get{ return _children.Count > 0;}}
		protected bool _isAutoDestroy;

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="childFailStop">是否子命令执行失败时停止？（停止后的状态是失败，否则是成功）</param>
		/// <param name="isAutoDestroy">执行完所有子命令后是否自动销毁</param>
		public CommandImmediately(bool childFailStop = true,bool isAutoDestroy = true):base()
		{
			this._isChildFailStop = childFailStop;
			_children = new LinkedList<CommandBase> ();
			this._isAutoDestroy = isAutoDestroy;
		}

		/// <summary>
		/// 添加子命令并直接执行
		/// </summary>
		/// <param name="command">Command.</param>
		public override void AddSubCommand(CommandBase command)
		{
			command.Parent = this;
			_children.AddLast (command);
            this.OnChildStart(command);
            command.Execute (_context);
		}

        public override void OnUpdate()
        {
            if (RunState == CmdRunState.Runing)
            {
                var node = _children.First;
                while (node != null)
                {
                    node.Value.OnUpdate();
                    node = node.Next;
                }
            }
        }

        public override void OnChildDone (CommandBase command)
		{
			this.On_ChildDoneCallback (command);
			if (_isChildFailStop)
			{
				this.State = command.State;
			}
			_children.Remove (command);
			OnChildDestroy (command);
			if (_isChildFailStop && this.State == CmdExecuteState.Fail)
			{
				Clear ();
				this.OnExecuteDone (CmdExecuteState.Fail);
			}
			else
			{
				if (_children.Count == 0)
				{
					this.OnExecuteDone (CmdExecuteState.Success);
				}
			}
		}

		protected override void OnExecuteDone (CmdExecuteState state)
		{
			this.State = state;
			OnExecuteFinish ();
			OnDoneInvoke ();

			if (Parent != null)
			{
				Parent.OnChildDone (this);
			}
			else if (_isAutoDestroy)
			{
				this.OnDestroy ();
			}
		}

		/// <summary>
		/// 清楚当前所有子命令
		/// </summary>
		public override void Clear ()
		{
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

