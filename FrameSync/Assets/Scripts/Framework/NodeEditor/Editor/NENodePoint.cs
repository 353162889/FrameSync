using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public enum NENodePointType
    {
        In,
        Out
    }

    public class NENodePoint
    {
        public Rect rect;
        public NENode node { get; private set; }
        public NENodePointType pointType { get; private set; }
        private GUIStyle m_cBtnStyle;
        public NENodePoint(NENode node, NENodePointType pointType)
        {
            this.node = node;
            this.pointType = pointType;
            this.rect = new Rect(0, 0, 40, 16);
            m_cBtnStyle = null;
        }

        public void Draw(Action<NENodePoint> onClickPoint)
        {
            if (m_cBtnStyle == null)
            {
                m_cBtnStyle = new GUIStyle((GUIStyle)"AppToolbar");
            }
            rect.x = node.rect.x + (node.rect.width - rect.width) / 2;
            switch (pointType)
            {
                case NENodePointType.In:
                    rect.y = node.rect.yMin - rect.height; 
                    break;
                case NENodePointType.Out:
                    rect.y = node.rect.yMax;
                    break;
            }
            if (GUI.Button(rect, "", m_cBtnStyle))
            {
                if (onClickPoint != null)
                {
                    onClickPoint(this);
                }
            }
        }
    }
}