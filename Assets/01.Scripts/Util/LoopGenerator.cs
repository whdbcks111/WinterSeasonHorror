using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class LoopGenerator : MonoBehaviour
{
    public float PerlinXScale = 58.349242f, PerlinYScale = 21.8761f;

    public SpriteRenderer LoopPrefab;
    public RandomSpriteInfo[] RandomSprites;
    public LoopSpriteInfo[] Exceptions;

    private ObjectPool<SpriteRenderer> _pool;
    private Vector3 _loopSize, _leftBottom, _rightTop;
    private readonly Dictionary<Vector2Int, SpriteRenderer> _loopObjectMap = new();
    private readonly Dictionary<Vector2Int, Sprite> _exceptionSprites = new();

    private float _totalWeight = 0f;

    private void Awake()
    {
        _pool = new(
            createFunc: () => Instantiate(LoopPrefab),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject)
            );
        _loopSize = LoopPrefab.bounds.size;

        foreach (var info in Exceptions)
        {
            _exceptionSprites[WorldToLoopPoint(info.Position.position)] = info.Sprite;
        }

        foreach(var info in RandomSprites)
        {
            _totalWeight += info.ChanceWeight;
        }
    }

    private void PlaceLoopObject(Vector2Int pos)
    {
        if (_loopObjectMap.ContainsKey(pos)) return;

        SpriteRenderer target = _pool.Get();
        if(_exceptionSprites.ContainsKey(pos))
        {
            target.sprite = _exceptionSprites[pos];
        }
        else
        {
            var randomValue = Mathf.PerlinNoise(pos.x * PerlinXScale, pos.y * PerlinYScale);
            if (randomValue >= 1) randomValue = 0f;

            randomValue *= _totalWeight;

            var current = 0f;
            foreach(var info in RandomSprites)
            {
                var next = current + info.ChanceWeight;
                if(current <= randomValue && randomValue < next)
                {
                    target.sprite = info.Sprite;
                    break;
                }
                current = next;
            }
        }
        target.transform.position = new(pos.x * _loopSize.x, pos.y * _loopSize.y);
        _loopObjectMap[pos] = target;
    }

    private void DestroyLoopObject(Vector2Int pos)
    {
        if (!_loopObjectMap.ContainsKey(pos)) return;

        SpriteRenderer target = _loopObjectMap[pos];
        _pool.Release(target);
        _loopObjectMap.Remove(pos);
    }

    private Vector2Int WorldToLoopPoint(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x / _loopSize.x), (int)(pos.y / _loopSize.y));
    }

    private void CheckFloor()
    {
        _leftBottom = Camera.main.ViewportToWorldPoint(new(0, 0));
        _rightTop = Camera.main.ViewportToWorldPoint(new(1, 1));

        var minPos = WorldToLoopPoint(_leftBottom) - Vector2Int.one * 2;
        var maxPos = WorldToLoopPoint(_rightTop) + Vector2Int.one * 2;

        var keys = _loopObjectMap.Keys.ToArray();
        foreach (var pos in keys)
        {
            if (pos.x < minPos.x || pos.x > maxPos.x || pos.y < minPos.y || pos.y > maxPos.y)
            {
                DestroyLoopObject(pos);
            }
        }

        for (int x = minPos.x; x <= maxPos.x; ++x)
        {
            for (int y = minPos.y; y <= maxPos.y; ++y)
            {
                PlaceLoopObject(new(x, y));
            }
        }
    }

    private void Update()
    {
        CheckFloor();
    }
}

[Serializable]
public class RandomSpriteInfo
{
    public Sprite Sprite;
    public float ChanceWeight;
}

[Serializable]
public class LoopSpriteInfo
{
    public Transform Position;
    public Sprite Sprite;
}