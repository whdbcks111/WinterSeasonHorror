using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Lever : InteractableObject
{


    [Range(0, 1f)] [SerializeField] private float duration = 1f;// 전환에 걸리는 시간 (초)
    [SerializeField] private float previousRotationZ; //바뀌기 이전 각도 값
    [SerializeField] private float interactRotationZ; //인터랙트 이후 각도 값
    [SerializeField] private Transform handleAxisTf; //핸들 축
    
    private bool _isOn = false; // 레버 상태
    public GameObject[] blockingObjects; // 길을 막는 오브젝트들
    public GameObject[] targetObjects;

    private bool flag = false;
    public override void OnInteract()
    {
        _isOn = !_isOn;
        HandleBlockingObjects(_isOn);
        HandleTargetObjects();
        
        // 기타 레버 상호작용 로직
    }
    protected override void ProvideVisualFeedback(bool isOn)
    {
        GetComponent<Light2D>().enabled = _isOn;
        StartCoroutine(RotateLever(duration, _isOn));
    }


    IEnumerator RotateLever(float time, bool isOn)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        var currentRot = handleAxisTf.eulerAngles.z;
        var targetRot = isOn ? interactRotationZ : previousRotationZ;
        

        //첫번째 방법
        for(float i = 0; i <= 1; i += Time.deltaTime / time)
        {
            handleAxisTf.eulerAngles = new(0, 0, Mathf.LerpAngle(
                currentRot, targetRot, i
            ));
            yield return null;
        }

        GetComponent<BoxCollider2D>().enabled = true;
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
    private void HandleTargetObjects()
    {
        foreach (var obj in targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!obj.activeSelf); // 레버가 활성화되면 Active 상태가 반대로 변화
            }
        }
    }
}
