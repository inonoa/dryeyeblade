using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField] ScoreCounter counter;
    [SerializeField] Text scoreText;
    int lastScore = 0;
    
    void Start()
    {
        counter.Score.Subscribe(score =>
        {
            if (score == 0) scoreText.text = "0";
            else            scoreText.DOCounter(lastScore, score, 1f);
        });
    }
}