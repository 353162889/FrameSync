using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NEConnection
    {
        public NENodePoint inPoint { get; private set; }
        public NENodePoint outPoint { get; private set; }
        public NEConnection(NENodePoint inPoint, NENodePoint outPoint)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
        }

        public void Draw(Action<NEConnection> onRemoveConnection)
        {
            Handles.DrawBezier(inPoint.rect.center, outPoint.rect.center,
                        inPoint.rect.center + Vector2.down * 50f, outPoint.rect.center + Vector2.up * 50f,
                        Color.white, null, 2f);

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                if (onRemoveConnection != null)
                {
                    onRemoveConnection(this);
                }
            }
        }
    }
}