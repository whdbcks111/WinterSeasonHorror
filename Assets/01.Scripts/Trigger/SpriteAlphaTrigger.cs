

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SpriteAlphaTrigger : BaseTrigger
{
    [Header("액티브 토글 대상 스프라이트들")]
    public SpriteRenderer[] TargetObjects;
    public float EnterAlpha = 1f, ExitAlpha = 0f, FadeTime = 0.5f;

    public override void Enter()
    {
        foreach (var obj in TargetObjects)
        {
            SetAlpha(obj, EnterAlpha).Forget();
        }
    }

    private async UniTask SetAlpha(SpriteRenderer renderer, float alpha)
    {
        var originalAlpha = renderer.color.a;
        Color col;
        for(float i = 0f; i <= 1f; i += Time.deltaTime / FadeTime)
        {
            if (renderer == null) return;
            col = renderer.color;
            col.a = Mathf.Lerp(originalAlpha, alpha, i);
            renderer.color = col;
            await UniTask.Yield();
            print(col.a);
        }
        col = renderer.color;
        col.a = alpha;
        renderer.color = col;
    }

    public override void Exit()
    {
        foreach (var obj in TargetObjects)
        {
            SetAlpha(obj, ExitAlpha).Forget();
        }
    }
}