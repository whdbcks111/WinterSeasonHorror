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
                // ���� ����Ŀ �̸� ã��
                int speakerIndex = script.participants.IndexOf(script.dialogues[i].speaker);
                if (speakerIndex == -1) speakerIndex = 0;

                // ��Ӵٿ� �޴��� ����Ŀ ����
                speakerIndex = EditorGUILayout.Popup("Speaker", speakerIndex, script.participants.ToArray());

                // ���õ� ����Ŀ�� dialogue ������Ʈ
                script.dialogues[i].speaker = script.participants[speakerIndex];
            }
        }

        // ��ũ��Ʈ ���� ���� ����
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }*/
}
