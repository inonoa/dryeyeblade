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
    [SerializeField] ScoreIncrementEffect scoreIncrementEffectPrefab;
    [SerializeField] ScoreViewBody body;
    
    void Start()
    {
        counter.Score.Subscribe(score => body.OnScoreSet(score));

        counter.ScoreAdded.Subscribe(info =>
        {
            Vector2 pos = (info.target as MonoBehaviour).transform.position;
            var effect = Instantiate(scoreIncrementEffectPrefab, pos + new Vector2(0.2f, 0.8f), Quaternion.identity);
            effect.Init(info.rate * info.target.Score);
        });
    }
}