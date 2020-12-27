using System.Linq;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;

public class ScoreIncrementEffect : MonoBehaviour
{
    [SerializeField, NativeFixedLength(10)]
    Sprite[] numSprite0to9;

    [SerializeField] SpriteRenderer plus;
    [SerializeField] SpriteRenderer[] numSpriteRenderersReverse;

    public void Init(int score)
    {
        char[] scoreChars = score.ToString().Reverse().ToArray();
        foreach (int i in Enumerable.Range(0, Mathf.Min(scoreChars.Length, numSpriteRenderersReverse.Length)))
        {
            numSpriteRenderersReverse[i].gameObject.SetActive(true);
            numSpriteRenderersReverse[i].sprite = numSprite0to9[int.Parse(scoreChars[i].ToString())];
        }

        plus.transform.position = numSpriteRenderersReverse[scoreChars.Length].transform.position;

        DOTween.Sequence()
            .Append(transform.DOMoveY(0.25f, 1f).SetRelative().SetEase(Ease.OutCubic))
            .onComplete += () => Destroy(gameObject);
    }
}