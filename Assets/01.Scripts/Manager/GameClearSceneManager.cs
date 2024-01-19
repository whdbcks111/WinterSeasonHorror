using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearSceneManager : MonoBehaviour
{
    public string TitleSceneName;
    public float SceneLoadDelay = 3f;

    public TextMeshProUGUI Text;
    public float TextAppearDelay = 2f;
    public float TextAppearTime = 1f; 

    private void Start()
    {
        LoadTitle().Forget();
    }

    private async UniTask LoadTitle()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(TextAppearDelay));
        Color textColor = Text.color;
        for(float i = 0; i <= 1f; i += Time.deltaTime / TextAppearTime)
        {
            textColor = Text.color;
            textColor.a = i;
            Text.color = textColor;
            await UniTask.Yield();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(SceneLoadDelay));
        for (float i = 1f; i >= 0f; i -= Time.deltaTime)
        {
            textColor = Text.color;
            textColor.a = i;
            Text.color = textColor;
            await UniTask.Yield();
        }

        textColor.a = 0;
        Text.color = textColor;

        await SceneManager.LoadSceneAsync(TitleSceneName);
    }
}
