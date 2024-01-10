using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class LoopGenerator : MonoBehaviour
{
    public SpriteRenderer LoopPrefab;

    private ObjectPool<SpriteRenderer> _pool;
    private Vector3 _loopSize, _leftBottom, _rightTop;
    private readonly Dictionary<Vector2Int, SpriteRenderer> _loopObjectMap = new();

    private void Awake()
    {
        _pool = new(
            createFunc: () => Instantiate(LoopPrefab),
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject)
            );
        _loopSize = LoopPrefab.bounds.size;
    }

    private void PlaceLoopObject(Vector2Int pos)
    {
        if (_loopObjectMap.ContainsKey(pos)) return;

        SpriteRenderer target = _pool.Get();
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

    private void CheckFloor()
    {
        _leftBottom = Camera.main.ViewportToWorldPoint(new(0, 0));
        _rightTop = Camera.main.ViewportToWorldPoint(new(1, 1));

        int minX = (int)(_leftBottom.x / _loopSize.x - 2), maxX = (int)(_rightTop.x / _loopSize.x + 2);
        int minY = (int)(_leftBottom.y / _loopSize.y - 2), maxY = (int)(_rightTop.y / _loopSize.y + 2);

        var keys = _loopObjectMap.Keys.ToArray();
        foreach (var pos in keys)
        {
            if (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY)
            {
                DestroyLoopObject(pos);
            }
        }

        for (int x = minX; x <= maxX; ++x)
        {
            for (int y = minY; y <= maxY; ++y)
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
