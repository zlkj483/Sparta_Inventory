using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif


[System.AttributeUsage(System.AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public ButtonAttribute(string label = null)
    {
        Label = label;
    }
}

#if UNITY_EDITOR


[CustomEditor(typeof(MonoBehaviour), true)]
[CanEditMultipleObjects]
public class ButtonAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
      
        base.OnInspectorGUI();


        var targetObject = target as MonoBehaviour;
        var type = targetObject.GetType();

   
        var methods = type.GetMethods(
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic);

        foreach (var method in methods)
        {
 
            var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttr == null)
                continue;

            string label = string.IsNullOrEmpty(buttonAttr.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : buttonAttr.Label;

   
            if (GUILayout.Button(label))
            {
        
                Undo.RecordObject(targetObject, "Button Invoke: " + method.Name);
                method.Invoke(targetObject, null);
                EditorUtility.SetDirty(targetObject);
            }
        }
    }
}

#endif
