using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerupBar : MonoBehaviour
{
    public static UIPowerupBar instance { get; private set; }

    public Image mask;
    float originalSize;

    void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        originalSize = mask.rectTransform.rect.width;
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
