using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace NodeEditor
{
    public class NETreeWindow : EditorWindow
    {
        
        private static float titleHeight = 20;
        private static float leftAreaWidth = 200;
        private static float rightAreaWidth = 200;

        public NECanvas canvas { get { return m_cCanvas; } }
        private NECanvas m_cCanvas;

        private int m_nTreeComposeIndex = 0;

        private List<Type> m_lstNodeType;
        private List<Type> m_lstNodeDataType;
        private GUIStyle m_cToolBarBtnStyle;
        private GUIStyle m_cToolBarPopupStyle;

        private NENode m_cRoot;

        [MenuItem("Tools/NETreeWindow")]
        static public void OpenWindow()
        {
            var window = EditorWindow.GetWindow<NETreeWindow>();
            window.titleContent = new GUIContent("NETreeWindow");
            var position = window.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            window.position = position;

            window.Show();
            window.FocusCanvasCenterPosition();
        }

        void OnEnable()
        {
            m_cToolBarBtnStyle = null;
            m_cToolBarPopupStyle = null;

            Load(NEConfig.arrTreeComposeData[m_nTreeComposeIndex]);
        }

        private void Load(NETreeComposeType containType)
        {
            NEData neData = null;
            if (m_cRoot != null)
            {
                foreach (var item in NEConfig.arrTreeComposeData)
                {
                    if (item.rootType == m_cRoot.node.GetType())
                    {
                        if (item == containType)
                        {
                            neData = GetCurrentTreeNEData();
                        }
                        break;
                    }
                }
                m_cRoot = null;
            }
            LoadByAttribute(containType.rootType, containType.lstNodeAttribute);

            //移除根节点
            List<Type> lst = new List<Type>();
            for (int i = 0; i < m_lstNodeType.Count; i++)
            {
                if (!IsRootType(m_lstNodeType[i])) lst.Add(m_lstNodeType[i]);
            }
            if (m_cCanvas != null) m_cCanvas.Dispose();
            m_cCanvas = new NECanvas(lst, CreateNENodeByNENodeType);
            CreateTreeByTreeData(neData);
        }

        public void FocusCanvasCenterPosition()
        {
            if (m_cCanvas != null)
            {
                float canvasWidth = position.width - leftAreaWidth - rightAreaWidth;
                float canvasHeight = position.height - titleHeight;
                Vector2 firstScrollPos = new Vector2((m_cCanvas.scrollViewRect.width - canvasWidth) / 2, (m_cCanvas.scrollViewRect.height - canvasHeight) / 2);
                m_cCanvas.scrollPos = firstScrollPos;
            }
        }

        private NENode CreateTreeByTreeData(NEData neData)
        {
            if (m_cCanvas != null) m_cCanvas.Clear();
            NENode node = null;
            if (neData == null)
            {
                var composeData = NEConfig.arrTreeComposeData[m_nTreeComposeIndex];
                object data = CreateNENodeByNENodeType(composeData.rootType);
                Vector2 center = m_cCanvas.scrollViewRect.center;
                node = m_cCanvas.CreateNode(center, data);
            }
            else
            {
                node = CreateNENode(neData);
            }
            m_cRoot = node;
            FocusCanvasCenterPosition();
            return node;
        }

        private NENode CreateNENode(NEData neData)
        {
            if (neData.data == null)
            {
                Debug.LogError("neData.data == null");
                return null;
            }
            object neNode = CreateNENodeByData(neData.data);
            NENode parentNode = m_cCanvas.CreateNode(neData.editorPos, neNode);
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    NENode childNode = CreateNENode(neData.lstChild[i]);
                    NEConnection connection = m_cCanvas.CreateConnect(parentNode, childNode);
                }
            }
            return parentNode;
        }

        private void LoadByAttribute(Type rootType, List<Type> types)
        {
            m_lstNodeType = new List<Type>();
            m_lstNodeDataType = new List<Type>();
            for (int i = 0; i < types.Count; i++)
            {
                var assembly = types[i].Assembly;
                var lstTypes = assembly.GetTypes();
                for (int j = 0; j < lstTypes.Length; j++)
                {
                    var arr = lstTypes[j].GetCustomAttributes(types[i], true);
                    if (arr.Length > 0)
                    {
                        m_lstNodeType.Add(lstTypes[j]);
                        var attr = arr[0] as NENodeAttribute;
                        m_lstNodeDataType.Add(attr.nodeDataType);
                    }
                }
            }
        }

        private bool IsRootType(Type type)
        {
            for (int i = 0; i < NEConfig.arrTreeComposeData.Length; i++)
            {
                if (type == NEConfig.arrTreeComposeData[i].rootType) return true;
            }
            return false;
        }

        private object CreateNENodeByNENodeType(Type neNodeType)
        {
            int index = m_lstNodeType.IndexOf(neNodeType);
            if (index == -1)
            {
                Debug.LogError("类型为:" + neNodeType + "的节点没有配置节点数据类型");
                return null;
            }
            Type dataType = m_lstNodeDataType[index];
            return CreateNENode(neNodeType, dataType, null);
        }

        private object CreateNENodeByData(object data)
        {
            Type dataType = data.GetType();
            int index = m_lstNodeDataType.IndexOf(dataType);
            if (index == -1)
            {
                Debug.LogError("类型为:" + dataType + "的数据没有配置节点类型");
                return null;
            }
            Type nodeType = m_lstNodeType[index];
            return CreateNENode(nodeType, dataType, data);
        }

        private object CreateNENode(Type neNodeType, Type neNodeDataType, object data = null)
        {
            object node = Activator.CreateInstance(neNodeType);
            var fieldInfos = neNodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var item in fieldInfos)
            {
                if (item.GetCustomAttributes(typeof(NENodeDataAttribute), true).Length > 0)
                {
                    if (data == null)
                    {
                        data = Activator.CreateInstance(neNodeDataType);
                    }
                    item.SetValue(node, data);
                    break;
                }
            }
            return node;
        }

        void OnDisable()
        {
            m_cCanvas.Dispose();
            m_cCanvas = null;
            m_cToolBarBtnStyle = null;
            m_cToolBarPopupStyle = null;
        }

        private int toolBarIndex = 0;
        private Vector3 leftScrollPos;
        private Vector3 rightScrollPos;
        void OnGUI()
        {
            if (m_cToolBarBtnStyle == null)
            {
                m_cToolBarBtnStyle = new GUIStyle((GUIStyle)"toolbarbutton");
            }

            if (m_cToolBarPopupStyle == null)
            {
                m_cToolBarPopupStyle = new GUIStyle((GUIStyle)"ToolbarPopup");
            }

            float centerAreaWidth = position.width - leftAreaWidth - rightAreaWidth;
            if (centerAreaWidth < 0) centerAreaWidth = 0;

            //画布整体描述区域
            Rect leftArea = new Rect(0, titleHeight, leftAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(leftArea);
            GUILayout.Label("总描述", m_cToolBarBtnStyle, GUILayout.Width(leftArea.width));
            leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, false, true);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //画布区域
            Rect centerArea = new Rect(leftArea.width, titleHeight, centerAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(centerArea);
            m_cCanvas.Draw(centerArea);
            GUILayout.EndArea();

            //单个节点描述区域
            Rect rightArea = new Rect(leftArea.width + centerAreaWidth, titleHeight, rightAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(rightArea);
            GUILayout.Label("节点描述", m_cToolBarBtnStyle, GUILayout.Width(rightArea.width));
            rightScrollPos = GUILayout.BeginScrollView(rightScrollPos, false, true);
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            if (m_cCanvas.selectNode != null && m_cCanvas.selectNode.dataProperty != null)
            {
                if (!string.IsNullOrEmpty(m_cCanvas.selectNode.desc))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("节点描述:");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    bool oldWordWrap = EditorStyles.textArea.wordWrap;
                    EditorStyles.textArea.wordWrap = true;
                    GUILayout.TextArea(m_cCanvas.selectNode.desc, GUILayout.Width(rightArea.width - 40), GUILayout.Height(60));
                    EditorStyles.textArea.wordWrap = oldWordWrap;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUIUtility.labelWidth = 50;
                NEDataProperties.Draw(m_cCanvas.selectNode.dataProperty, GUILayout.Width(rightArea.width - 50));
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //标题区域
            Rect titleRect = new Rect(0, 0, position.width, titleHeight);
            m_cToolBarBtnStyle.fixedHeight = titleRect.height;
            m_cToolBarPopupStyle.fixedHeight = titleRect.height;
            GUILayout.BeginArea(titleRect);
            //GUILayout.Label("", tt,GUILayout.Width(50),GUILayout.Height(20));
            GUILayout.BeginHorizontal();
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
            int oldTreeComposeIndex = m_nTreeComposeIndex;
            m_nTreeComposeIndex = EditorGUILayout.Popup(m_nTreeComposeIndex, NEConfig.arrTreeComposeTypeDesc, m_cToolBarPopupStyle, GUILayout.Width(100));
            if (oldTreeComposeIndex != m_nTreeComposeIndex)
            {
                Load(NEConfig.arrTreeComposeData[m_nTreeComposeIndex]);
            }
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(position.width - 10 - 100 - 50 - 50 - 50 - 10));
            if (GUILayout.Button("创建", m_cToolBarBtnStyle, GUILayout.Width(50))) { CreateTreeByTreeData(null); }
            if (GUILayout.Button("加载", m_cToolBarBtnStyle, GUILayout.Width(50))) { LoadTreeByTreeData(); }
            if (GUILayout.Button("保存", m_cToolBarBtnStyle, GUILayout.Width(50))) { SaveTreeToTreeData(); }
            GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (GUI.changed) Repaint();
        }

        private void LoadTreeByTreeData()
        {
            string path = EditorUtility.OpenFilePanel("加载数据", Application.dataPath, "bytes");
            if (path.Length != 0)
            {
                //通过前后缀确定当前数据是哪种类型,需要先切换到当前类型，在加载数据，否则数据有可能不对
                NEData neData = NEUtil.DeSerializerObject(path, typeof(NEData), m_lstNodeDataType.ToArray()) as NEData;
                CreateTreeByTreeData(neData);
            }
        }

        private void SaveTreeToTreeData()
        {
            if (m_nTreeComposeIndex < 0 || m_nTreeComposeIndex > NEConfig.arrTreeComposeData.Length)
            {
                Debug.Log("需要选择树的类型");
                return;
            }
            var composeData = NEConfig.arrTreeComposeData[m_nTreeComposeIndex];
            NEData data = GetCurrentTreeNEData();
            if (data == null)
            {
                Debug.Log("没有树数据");
                return;
            }
            string path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath, "", composeData.fileExt);
            if (path.Length != 0)
            {
                NEUtil.SerializerObject(path, data, m_lstNodeDataType.ToArray());
            }
            AssetDatabase.Refresh();
        }

        private void DebugNEData(NEData neData)
        {
            Debug.Log(neData.editorPos + "," + neData.data.GetType());
            if (neData.lstChild != null)
            {
                for (int i = 0; i < neData.lstChild.Count; i++)
                {
                    DebugNEData(neData.lstChild[i]);
                }
            }
        }

        private NEData GetCurrentTreeNEData()
        {
            if (m_cRoot == null) return null;
            var lstConnection = m_cCanvas.lstConnection;
            List<NENode> handNodes = new List<NENode>();
            NEData neData = GetNodeNEData(m_cRoot, lstConnection, handNodes);
            return neData;
        }

        private NEData GetNodeNEData(NENode node, List<NEConnection> lst, List<NENode> handNodes)
        {
            if (handNodes.Contains(node))
            {
                Debug.LogError("树的连线进入死循环，节点=" + node.node.GetType());
                return null;
            }
            handNodes.Add(node);

            INENode neNode = node.node as INENode;
            NEData neData = new NEData();
            neData.data = neNode.data;
            neData.editorPos = node.rect.center;

            List<NENode> lstSubNode = new List<NENode>();
            for (int i = 0; i < lst.Count; i++)
            {
                NEConnection connection = lst[i];
                if (connection.outPoint.node == node)
                {
                    NENode childNode = connection.inPoint.node;
                    lstSubNode.Add(childNode);
                }
            }
            lstSubNode.Sort(NodeSort);
            for (int i = 0; i < lstSubNode.Count; i++)
            {
                NENode childNode = lstSubNode[i];
                NEData childNEData = GetNodeNEData(childNode, lst, handNodes);
                if (neData.lstChild == null) neData.lstChild = new List<NEData>();
                neData.lstChild.Add(childNEData);
            }

            return neData;
        }

        //按照位置排序
        private int NodeSort(NENode a, NENode b)
        {
            int res = 0;
            if (a.rect.center.x - b.rect.center.x > 0) res = 1;
            else if (a.rect.center.x - b.rect.center.x < 0) res = -1;
            return res;
        }
    }

}