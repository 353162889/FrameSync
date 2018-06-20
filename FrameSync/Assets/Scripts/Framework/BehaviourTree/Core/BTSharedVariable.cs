using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public abstract class BTSharedVariable
    {
        public string Name { get; set; }
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }
    public class BTSharedVariable<T> : BTSharedVariable
    {
        [SerializeField]
        protected T mValue;
        protected BTSharedVariable() { mValue = default(T); }
        public T Value { get; set; }
        public override object GetValue() { return mValue; }
        public override void SetValue(object value)
        {
            if(value != (object)mValue)
            {
                mValue = (T)value;    
            }
        }
    }
}
