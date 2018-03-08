using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 并行所有子对象，子对象只要有一个完成，父对象就完成，只要有一个失败，父对象失败
    /// </summary>
    public class CommandParallelOnce : CommandContainerBase
    {
        protected List<CommandBase> _children;

        public CommandParallelOnce():base()
		{
            _children = new List<CommandBase>();
        }

        public override void AddSubCommand(CommandBase command)
        {
            command.Parent = this;
            _children.Add(command);
        }

        public override void Execute(ICommandContext context)
        {
            base.Execute(context);
            int count = _children.Count;
            for (int i = 0; i < count; i++)
            {
                _children[i].Execute(context);
            }
        }

        public override void OnUpdate()
        {
            if (RunState == CmdRunState.Runing)
            {
                int count = _children.Count;
                for (int i = 0; i < count; i++)
                {
                    _children[i].OnUpdate();
                }
            }
        }

        public override void OnChildDone(CommandBase command)
        {
            base.OnChildDone(command);
            if (command.State == CmdExecuteState.Success)
            {
                this.OnExecuteDone(CmdExecuteState.Success);
            }
            else
            {
                this.OnExecuteDone(CmdExecuteState.Fail);
            }
        }

        public override void Clear()
        {
            int count = _children.Count;
            for (int i = 0; i < count; i++)
            {
                _children[i].OnDestroy();
            }
            _children.Clear();
        }

        public override void OnDestroy()
        {
            Clear();
            base.OnDestroy();
        }
    }
}
