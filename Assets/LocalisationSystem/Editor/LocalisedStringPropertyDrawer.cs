using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Localization_System.Editor
{
    [CustomPropertyDrawer(typeof(LocalisedString))]
    public class LocalisedStringPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            LocalisedString self = (LocalisedString)GetParent(property.FindPropertyRelative("ID"));
            Rect enumPopupRect = new Rect(position.x, position.y, position.width -100, position.height);
            Rect textAssetRect = new Rect(position.x + (self.stringFamily? position.width-100 : 0), position.y, position.width - (self.stringFamily? position.width - 100 : 0), position.height);

            
            if (self.stringFamily != null)
            {
                var s = self.stringFamily.family;
                string[] ids = new string[s.Count];
            
                for (int i = 0; i < ids.Length; i++)
                {
                    ids[i] = s[i].strings[0];
                }
                
                property.FindPropertyRelative("ID").intValue = EditorGUI.Popup(enumPopupRect,label.text,
                    property.FindPropertyRelative("ID").intValue, ids);
            }
            self.stringFamily = EditorGUI.ObjectField(textAssetRect, self.stringFamily ? "" : label.text, self.stringFamily, typeof(StringFamily)) as StringFamily;
        }
        
        
        
        public object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements.Take(elements.Length-1))
            {
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }
        
        public object GetValue(object source, string name)
        {
            if(source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }
        
        public object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while(index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
        
        
    }
}