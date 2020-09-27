using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI (Rect pos, SerializedProperty property, GUIContent label){
        HexCoordinates coordinates = new HexCoordinates(property.FindPropertyRelative("x").intValue, property.FindPropertyRelative("z").intValue);
        pos = EditorGUI.PrefixLabel(pos, label);
        GUI.Label(pos, coordinates.ToString());
    }
}
