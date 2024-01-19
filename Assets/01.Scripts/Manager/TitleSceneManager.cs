using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public string GameSceneName;
    public Image BlackScreen;

    private bool _isLoading = false;

    private void Awake()
    {
        BlackScreen.color = Color.black;
        FadeBlack().Forget();
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButton(0))
        {
            LoadGame().Forget();
        }
    }

    private async UniTask FadeBlack()
    {
        for(float i = 1f; i >= 0f; i -= Time.deltaTime)
        {
            BlackScreen.color = new Color(0, 0, 0, i);
            await UniTask.Yield();
        }
        BlackScreen.color = new Color(0, 0, 0, 0);
    }

    private async UniTask LoadGame()
    {
        if (_isLoading) return;
        _isLoading = true;

        await SceneManager.LoadSceneAsync(GameSceneName);
    }
}
