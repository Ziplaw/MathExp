using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIBuilder_test.Editor
{
    [CustomPropertyDrawer(typeof(CustomObject))]
    public class CustomObjectPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // return base.CreatePropertyGUI(property);
            var container = new VisualElement();
            
            var tfProp = new PropertyField(property.FindPropertyRelative("tf"));
            var valueProp = new PropertyField(property.FindPropertyRelative("value"));

            container.Add(tfProp);
            container.Add(valueProp);

            return container;
        }
    }
}