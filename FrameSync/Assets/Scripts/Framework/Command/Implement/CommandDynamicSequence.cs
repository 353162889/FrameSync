using System;

namespace Framework
{
	public class CommandDynamicSequence : CommandSequence
	{
		protected bool _isAutoDestroy;

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="childFailStop">是否子命令执行失败时停止？（停止后的状态是失败，否则是成功）</param>
		/// <param name="isAutoDestroy">执行完所有子命令后是否自动销毁</param>
		public CommandDynamicSequence(bool childFailStop = true,bool isAutoDestroy = true)
            :base(childFailStop)
		{
			this._isAutoDestroy = isAutoDestroy;
		}

		/// <summary>
		/// 添加子命令，如果不在执行中，调用Execute执行当前添加的子命令，如果在执行中，添加到队列中
		/// </summary>
		/// <param name="command">Command.</param>
		public override void AddSubCommand (CommandBase command)
		{
			base.AddSubCommand (command);
			Execute ();
		}

		public override void Execute (ICommandContext context)
		{
			if (!this.IsExecuting)
			{
				base.Execute (context);
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
	}
}

