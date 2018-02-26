using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public interface IServerMsg
    {
        void HandleMsg(object msg);
    }
    public abstract class MsgBase<T> : IServerMsg
    {
        public void HandleMsg(object msg)
        {
            T t = (T)msg;
            if (t != null)
            {
                HandleMsg(t);
            }
        }

        protected abstract void HandleMsg(T msg);
    }
}
