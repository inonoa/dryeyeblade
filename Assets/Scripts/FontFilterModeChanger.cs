using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class FontFilterModeChanger : MonoBehaviour
{
    [Button]
    void Change()
    {
        GetComponentsInChildren<Text>().ForEach(text =>
        {
            text.font.material.mainTexture.filterMode = FilterMode.Point;
        });
    }

    void Awake()
    {
        Change();
    }
}