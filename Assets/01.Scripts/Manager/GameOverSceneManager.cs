using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    public int GameSceneIdx = 0;

    private bool _isLoading = false;

    private void Update()
    {
        if(Input.anyKey)
        {
            LoadGame().Forget();
        }
    }

    private async UniTask LoadGame()
    {
        if (_isLoading) return;
        _isLoading = true;

        await SceneManager.LoadSceneAsync(GameSceneIdx);
        await UniTask.WaitUntil(() => SaveManager.Instance != null);
        await UniTask.Yield();
        SaveManager.Instance.LoadGameData();
    }
}
