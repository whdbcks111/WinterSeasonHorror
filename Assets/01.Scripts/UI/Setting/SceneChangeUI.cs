using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeUI : MonoBehaviour
{
    public string SceneName;
    public bool LoadAsync = true;
    private bool _isLoading = false;

    public void ChangeScene()
    {
        if(_isLoading) return;
        _isLoading = true;
        if(LoadAsync)
        {
            SceneManager.LoadSceneAsync(SceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
