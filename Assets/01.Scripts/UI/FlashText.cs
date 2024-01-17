using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FlashText : MonoBehaviour
{
    public float FlashRate;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _text.color = SetAlpha(_text.color, Mathf.Repeat(Time.time, FlashRate * 2) > FlashRate ? 1f : 0f);
    }

    private Color SetAlpha(Color col, float alpha)
    {
        col.a = alpha;
        return col;
    }
}
