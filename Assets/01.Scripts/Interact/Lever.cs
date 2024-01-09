using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : InteractableObject
{
    private bool isOn = false; // 레버 상태
    public GameObject[] blockingObjects; // 길을 막는 오브젝트들

    public override void OnInteract()
    {
        isOn = !isOn;
        HandleBlockingObjects(isOn);
        base.ProvideVisualFeedback(isOn);
        // 기타 레버 상호작용 로직
    }
    

    private void HandleBlockingObjects(bool active)
    {
        foreach (var obj in blockingObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!active); // 레버가 활성화되면 오브젝트 비활성화
            }
        }
    }
}
