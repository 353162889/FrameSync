using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NodeEditor
{
    public class NEDataProperties
    {
        public static void Draw(System.Object obj, params GUILayoutOption[] options)
        {
            Draw(GetProperties(obj), options);
        }

        public static void Draw(NEDataProperty[] properties,params GUILayoutOption[] options)
        {
            if (properties == null) return;
            EditorGUILayout.BeginVertical(options);

            foreach (NEDataProperty property in properties)
            {
                object oldValue = property.GetValue();
                object newValue = null;
                switch (property.Type)
                {
                    case NEDatapRropertyType.Integer:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Space(3);
                         newValue = EditorGUILayout.IntField((int)oldValue, options);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case NEDatapRropertyType.Float:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Space(3);
                        newValue = EditorGUILayout.FloatField((float)oldValue , options);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case NEDatapRropertyType.Boolean:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Space(3);
                        newValue = EditorGUILayout.Toggle((bool)oldValue, options);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case NEDatapRropertyType.String:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Space(3);
                        newValue = EditorGUILayout.TextField((String)oldValue, options);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case NEDatapRropertyType.Vector2:
                        newValue = EditorGUILayout.Vector2Field(property.Name, (Vector2)oldValue, options);
                        break;
                    case NEDatapRropertyType.Vector3:
                        newValue = EditorGUILayout.Vector3Field(property.Name, (Vector3)oldValue, options);
                        break;
                    case NEDatapRropertyType.Enum:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        Space(3);
                        newValue = EditorGUILayout.EnumPopup((Enum)oldValue, options);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case NEDatapRropertyType.Array:
                        EditorGUILayout.BeginHorizontal();
                        property.isShow = EditorGUILayout.Foldout(property.isShow, property.Name);
                        EditorGUILayout.EndHorizontal();
                        newValue = property.GetValue();
                        if (property.isShow)
                        {
                            EditorGUILayout.BeginHorizontal();
                            Space(3);
                            int oldLen = property.GetArrayLen();
                            int newLen = EditorGUILayout.IntField("Len:", oldLen, options);
                            if (oldLen != newLen)
                            {
                                property.SetArrayLen(newLen);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            Space(3);
                            var array = newValue as Array;
                           
                            if (array != null)
                            {
                                DrawArray(array,options);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        break;
                    default:
                        break;
                }

                if (oldValue != newValue)
                    property.SetValue(newValue);
            }

            EditorGUILayout.EndVertical();
        }

        public static void DrawArray(Array array, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical();
            //Space(3);
            Type type = array.GetType().GetElementType();
            for (int i = 0; i < array.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var oldValue = array.GetValue(i);
                if (type == typeof(int))
                {
                    var newValue = EditorGUILayout.IntField(i.ToString(), (int)oldValue, options);
                    if(newValue != (int)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type == typeof(float))
                {
                    var newValue = EditorGUILayout.FloatField(i.ToString(), (float)oldValue, options);
                    if (newValue != (float)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type == typeof(bool))
                {
                    var newValue = EditorGUILayout.Toggle(i.ToString(), (bool)oldValue, options);
                    if (newValue != (bool)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type == typeof(string))
                {
                    var newValue = EditorGUILayout.TextField(i.ToString(), (string)oldValue, options);
                    if (newValue != (string)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type == typeof(Vector2))
                {
                    var newValue = EditorGUILayout.Vector2Field(i.ToString(), (Vector2)oldValue, options);
                    if (newValue != (Vector2)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type == typeof(Vector3))
                {
                    var newValue = EditorGUILayout.Vector3Field(i.ToString(), (Vector3)oldValue, options);
                    if (newValue != (Vector3)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                else if (type.IsEnum)
                {
                    var newValue = EditorGUILayout.EnumPopup(i.ToString(), (Enum)oldValue, options);
                    if (newValue != (Enum)oldValue)
                    {
                        array.SetValue(newValue, i);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private static void Space(int count)
        {
            for (int i = 0; i < count; i++)
            {
                EditorGUILayout.Space();
            }
        }

        public static NEDataProperty[] GetProperties(System.Object obj)
        {
            List<NEDataProperty> properties = new List<NEDataProperty>();
            FieldInfo[] infos = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo info in infos)
            {
                NEDatapRropertyType type = NEDatapRropertyType.Integer;
                if (NEDataProperty.GetPropertyType(info, out type))
                {
                    NEDataProperty property = new NEDataProperty(obj, info, type);
                    properties.Add(property);
                }
            }

            return properties.ToArray();
        }
    }

    public enum NEDatapRropertyType
    {
        Generic,
        Integer,
        Float,
        Boolean,
        String,
        Vector2,
        Vector3,
        Enum,
        Array,
    }

    public class NEDataProperty
    {
        private System.Object m_Instance;
        private FieldInfo m_Info;
        private NEDatapRropertyType m_Type;
        public bool isShow { get; set; }

        public NEDatapRropertyType Type
        {
            get { return m_Type; }
        }

        private string m_sName;

        public string Name
        {
            get { return m_sName; }
        }

        private bool m_bShowOnNode;
        public bool showOnNode { get { return m_bShowOnNode; } }

        public NEDataProperty(System.Object instance, FieldInfo info, NEDatapRropertyType propertyType)
        {
            m_Instance = instance;
            m_Info = info;
            m_Type = propertyType;
            //m_sName = ObjectNames.NicifyVariableName(m_Info.Name);
            m_sName = m_Info.Name;
            m_bShowOnNode = false;
            var arr = m_Info.GetCustomAttributes(typeof(NEPropertyAttribute), false);
            if (arr.Length > 0)
            {
                NEPropertyAttribute propertyAttr= (NEPropertyAttribute)arr[0];
                m_sName = propertyAttr.name;
                m_bShowOnNode = propertyAttr.showOnNode;
            }
            if (propertyType == NEDatapRropertyType.Array)
            {
                isShow = false;
            }
            else
            {
                isShow = true;
            }
        }

        public System.Object GetValue()
        {
            return m_Info.GetValue(m_Instance);
        }

        public void SetValue(System.Object obj)
        {
            m_Info.SetValue(m_Instance, obj);
        }

        public int GetArrayLen()
        {
            if (m_Type != NEDatapRropertyType.Array) return 0;
            var value = GetValue();
            if (value == null) return 0;
            return ((Array)value).Length;
        }

        public void SetArrayLen(int len)
        {
            Array array = GetValue() as Array;
            if (array == null)
            {
                array = Activator.CreateInstance(m_Info.FieldType, new object[] { len }) as Array;
                SetValue(array);
            }
            else
            {
                if(array.Length != len)
                {
                    var newArray = array = Activator.CreateInstance(m_Info.FieldType, new object[] { len }) as Array;
                    int copyLen = len > array.Length ? array.Length : len;
                    Array.Copy(array, newArray, copyLen);
                    SetValue(newArray);
                }
            }
        }

        public static bool GetPropertyType(FieldInfo info, out NEDatapRropertyType propertyType)
        {
            propertyType = NEDatapRropertyType.Generic;
            Type type = info.FieldType;
            if (type == typeof(int))
            {
                propertyType = NEDatapRropertyType.Integer;
                return true;
            }

            if (type == typeof(float))
            {
                propertyType = NEDatapRropertyType.Float;
                return true;
            }

            if (type == typeof(bool))
            {
                propertyType = NEDatapRropertyType.Boolean;
                return true;
            }

            if (type == typeof(string))
            {
                propertyType = NEDatapRropertyType.String;
                return true;
            }

            if (type == typeof(Vector2))
            {
                propertyType = NEDatapRropertyType.Vector2;
                return true;
            }

            if (type == typeof(Vector3))
            {
                propertyType = NEDatapRropertyType.Vector3;
                return true;
            }

            if (type.IsEnum)
            {
                propertyType = NEDatapRropertyType.Enum;
                return true;
            }
            //只支持1维数组
            if (type.IsArray && type.GetArrayRank() == 1)
            {
                propertyType = NEDatapRropertyType.Array;
                return true;
            }

            return false;
        }

        //public static bool GetPropertyType(Type type, out NEDatapRropertyType propertyType)
        //{

        //}
    }
}