using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameManager grid = (GameManager)target;
        base.OnInspectorGUI();
        grid.w = (int)EditorGUILayout.Slider("w", grid.w, 1, 20);
        grid.h = (int)EditorGUILayout.Slider("h", grid.h, 1, 20);
    }
}