using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    public GameObject SettingUIPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SettingUIPanel.SetActive(SettingUIPanel.activeSelf);
        }
    }
}
