using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChatScript))]
public class ChatScriptEditor : Editor
{
    /*public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChatScript script = (ChatScript)target;

        if (script.participants.Count > 0)
        {
            for (int i = 0; i < script.dialogues.Count; i++)
            {
                // 현재 스피커 이름 찾기
                int speakerIndex = script.participants.IndexOf(script.dialogues[i].speaker);
                if (speakerIndex == -1) speakerIndex = 0;

                // 드롭다운 메뉴로 스피커 선택
                speakerIndex = EditorGUILayout.Popup("Speaker", speakerIndex, script.participants.ToArray());

                // 선택된 스피커로 dialogue 업데이트
                script.dialogues[i].speaker = script.participants[speakerIndex];
            }
        }

        // 스크립트 변경 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }*/
}
