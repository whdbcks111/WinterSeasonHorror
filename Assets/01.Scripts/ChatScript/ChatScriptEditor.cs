using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChatScript))]
public class ChatScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChatScript script = (ChatScript)target;
        if (GUILayout.Button("Load CSV for Chapter"))
        {
            string filePath = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                var (dialogues, participants) = CSVParser.ParseCSVForChapter(filePath, script.chapter);
                script.dialogues = dialogues;
                script.participants = new List<SpeakerType>(participants);
                EditorUtility.SetDirty(script);
            }
        }
    }
}