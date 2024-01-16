using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

// Source - https://github.com/SeksitSeeton/Auto-add-Shadow-Caster-2D-on-TileMap/blob/main/Assets/ShadowCaster2DTileMap.cs
// 원본 소스를 갇힌 타일맵에서도 적용되게끔 변경했음 (Generate()) 원본 메서드는 GenerateDefault()

[RequireComponent(typeof(CompositeCollider2D))]
public class TilemapShadowCasterGenerator : MonoBehaviour
{

    [SerializeField]
    private bool _selfShadows = true;

    private CompositeCollider2D _tilemapCollider;
    private List<Vector2> _unionVertices = new();


    static readonly FieldInfo _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo _generateShadowMeshMethod = typeof(ShadowCaster2D)
            .Assembly
            .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
            .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    [ContextMenu("GenerateDefault")]
    public void GenerateDefault()
    {
        DestroyAllChildren();

        _tilemapCollider = GetComponent<CompositeCollider2D>();

        for (int i = 0; i < _tilemapCollider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[_tilemapCollider.GetPathPointCount(i)];
            _tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this._selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            _shapePathField.SetValue(shadowCasterComponent, testPath);
            _meshField.SetValue(shadowCasterComponent, new Mesh());
            _generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { _meshField.GetValue(shadowCasterComponent), _shapePathField.GetValue(shadowCasterComponent) });
        }

    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        _tilemapCollider = GetComponent<CompositeCollider2D>();

        if (_tilemapCollider.pathCount != 2)
        {
            print("Shadow must be used in one closed tiles. Please erase the other tiles to other Tilemap.");
            return;
        }

        _unionVertices.Clear();
        DestroyAllChildren();


        Vector2[] pathVertices = new Vector2[_tilemapCollider.GetPathPointCount(0)];
        _tilemapCollider.GetPath(0, _unionVertices);
        _tilemapCollider.GetPath(1, pathVertices);
        _unionVertices.Add(_unionVertices[0]);

        var index = 0;
        var endPath = _unionVertices[0];
        var length = Vector2.Distance(pathVertices[0], endPath);

        for (var i = 1; i < pathVertices.Length; i++)
        {
            var path = pathVertices[i];
            var curLen = Vector2.Distance(endPath, path);
            if (curLen < length)
            {
                length = curLen;
                index = i;
            }
        }

        for (var i = 0; i < pathVertices.Length + 1; i++)
        {
            var path = pathVertices[(index + i) % pathVertices.Length];
            _unionVertices.Add(path);
        }

        var shadowCaster = new GameObject("ShadowCaster");
        shadowCaster.transform.parent = gameObject.transform;
        shadowCaster.transform.position = Vector3.zero;
        ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<ShadowCaster2D>();
        shadowCasterComponent.selfShadows = this._selfShadows;

        var testPath = new Vector3[_unionVertices.Count];
        var j = 0;
        foreach (var path in _unionVertices)
        {
            testPath[j++] = path;
        }

        _shapePathField.SetValue(shadowCasterComponent, testPath);
        _meshField.SetValue(shadowCasterComponent, new Mesh());
        _generateShadowMeshMethod.Invoke(shadowCasterComponent,
                new object[] {
                    _meshField.GetValue(shadowCasterComponent),
                    _shapePathField.GetValue(shadowCasterComponent)
                }
        );

    }
    public void DestroyAllChildren()
    {

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }

    }

}
