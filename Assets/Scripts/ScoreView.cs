using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UniRx;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField] ScoreCounter counter;
    [SerializeField] Image numImagePrefab;
    [SerializeField] Image firstDigitImage;
    [SerializeField] Vector3 gap;
    [SerializeField, NativeFixedLength(10)] Sprite[] numberSprites;
    [SerializeField] new AudioSource audio;
    [SerializeField] ScoreIncrementEffect scoreIncrementEffectPrefab;
    
    List<Image> numImages = new List<Image>();
    
    [SerializeField, ReadOnly] int scoreInView = 0;
    
    void Start()
    {
        firstDigitImage.sprite = numberSprites[0];
        numImages.Add(firstDigitImage);
        
        counter.Score.Subscribe(score =>
        {
            if (score == 0)
            {
                numImages.Skip(1).ForEach(numImg => Destroy(numImg.gameObject));
                numImages[0].sprite = numberSprites[0];
                numImages = numImages.Take(1).ToList();
                scoreInView = 0;
            }
            else
            {
                float duration = Mathf.Sqrt(score - scoreInView) / 10f;
                DOTween.To
                (
                    () => scoreInView,
                    SetNumber,
                    score,
                    duration
                )
                .SetEase(Ease.Linear);

                audio.clip = SoundDatabase.Instance.scoreUp;
                audio.loop = true;
                audio.volume = 0.6f;
                DOVirtual.DelayedCall(0.2f,             () => audio.Play());
                DOVirtual.DelayedCall(duration - 0.05f, () => audio.Stop());
            }
        });

        counter.ScoreAdded.Subscribe(info =>
        {
            Vector2 pos = (info.target as MonoBehaviour).transform.position;
            var effect = Instantiate(scoreIncrementEffectPrefab, pos + new Vector2(0.2f, 0.8f), Quaternion.identity);
            effect.Init(info.rate * info.target.Score);
        });
    }

    void SetNumber(int newVal)
    {
        char[] valChars = newVal.ToString().Reverse().ToArray();
        foreach (var i in Enumerable.Range(0, valChars.Length))
        {
            if (i >= numImages.Count)
            {
                var newNum = Instantiate(numImagePrefab, this.transform);
                newNum.transform.localPosition = firstDigitImage.transform.localPosition + i * gap;
                numImages.Add(newNum);
            }
            numImages[i].sprite = numberSprites[int.Parse(valChars[i].ToString())];
        }

        scoreInView = newVal;
    }
}