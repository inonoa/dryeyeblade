using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreViewSimple : MonoBehaviour
{
    [SerializeField] Image numImagePrefab;
    [SerializeField] Vector3 gap;
    [SerializeField, NativeFixedLength(10)] Sprite[] numberSprites;

    List<Image> images;

    [Button]
    public void Set(int score)
    {
        if (images != null)
        {
            images.ForEach(img => Destroy(img.gameObject));
        }
        images = new List<Image>();

        // 絶対もっといい方法ある
        score.ToString().Select(digitStr => int.Parse(digitStr.ToString())).ForEach((digit, i) =>
        {
            var digitImage = Instantiate(numImagePrefab, transform);
            digitImage.transform.localPosition = i * gap;
            digitImage.sprite = numberSprites[digit];
            images.Add(digitImage);
        });
    }
}