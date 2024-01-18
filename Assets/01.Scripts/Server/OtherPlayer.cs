using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class OtherPlayer : MonoBehaviour
{
    
    public GameObject handLight;
    public string userId;
    private PlayerData _playerData;
    private SpriteRenderer _spriteRenderer;

    

    Sprite GetSpriteByName(string name)
    {
        if (ServerManager.Instance.spriteDictionary.TryGetValue(name, out Sprite sprite))
        {
            return sprite;
        }
        return null;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RefreshData(PlayerData data)
    {
        if (data.playerID != userId)
        {
            Debug.Log("데이터 매칭이 잘못됐습니다." + userId + "와 " + data.playerID + "가 다릅니다.");
            return;
        }
        else
        {
            var position = new Vector3(data.position[0], data.position[1], data.position[2]);
            transform.position = position;
            _spriteRenderer.flipX = data.isFlipX;
            _spriteRenderer.sprite = GetSpriteByName(data.spriteName);
            //Debug.Log(GetSpriteByName(data.spriteName));
            //Debug.Log(_spriteRenderer.sprite.name + GetSpriteByName(data.spriteName).name);
            handLight.SetActive(data.lightActive);
        }
        
    }
}
