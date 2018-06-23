using Framework;
using NodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BTCore
{
    public enum BTResult
    {
        Success,
        Running,
        Failure,
    }
    abstract public class BTNode : INENode{
        [NENodeData]
        protected object m_cData;
        public object data {  get { return m_cData; }  set { m_cData = value; OnInitData(m_cData); } }

        protected virtual void OnInitData(object data) { }

        public virtual BTResult OnTick(BTBlackBoard blackBoard){ return BTResult.Success; }

        public virtual void AddChild(BTNode child) { CLog.LogError(this.GetType() + " can not AddChild!"); }

        public virtual void Clear() { }
    }
}
