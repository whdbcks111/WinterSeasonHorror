

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : BaseTrigger
{
    public bool LoadAsync = true;
    public string SceneName;

    public override void Enter()
    {
        if (LoadAsync)
        {
            SceneManager.LoadSceneAsync(SceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneName);
        }
    }

    public override void Exit()
    {
    }
}