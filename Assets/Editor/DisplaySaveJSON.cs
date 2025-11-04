using UnityEngine;
using UnityEditor;
public class DisplaySaveJSON : EditorWindow
{
    public string content = "Nothing to display.. edit this value to show stuff!";
    private Vector2 scrollPos;

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.TextArea(content, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }
}
