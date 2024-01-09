using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelUI : MonoBehaviour
{
    public UIManager.ScreenFit fitType;
    public Image image;
    public bool flag = false;
    [Range(0f, 3f)] public float displayImageTime = 1f; // 이미지가 사라지는 데 걸리는 시간
    [Range(0f, 3f)] public float fadeOutTime = 2.0f; // 이미지가 사라지는 데 걸리는 시간

}
