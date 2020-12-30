using System;
using System.Linq;
using System.Runtime.InteropServices;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    [SerializeField] ScoreCounter scoreCounter;

    [Space(10)]
    [SerializeField] Image BG;
    
    [SerializeField] Animator dead;
    [SerializeField] float[] deadSoundDelays;
    [SerializeField] new AudioSource audio;
    
    [SerializeField] ScoreViewBody scoreViewBody;
    
    [SerializeField] Button restartButton;
    [SerializeField] Button rankingButton;
    [SerializeField] Button tweetButton;
    
    [SerializeField] Animator blind;

    [SerializeField, Tooltip("`[score]`でスコアが入るよ"), Multiline] string tweetText;
    [SerializeField] RankingPopup ranking;

    Subject<Unit> _Restart = new Subject<Unit>();

    void Awake()
    {
        restartButton.OnClickAsObservable()
            .Subscribe(_ => OnRestart());

        tweetButton.OnClickAsObservable().Subscribe(_ =>
        {
            Tweet(tweetText.Replace("[score]", scoreCounter.Score.Value.ToString()));
        });

        rankingButton.OnClickAsObservable().Subscribe(_ =>
        {
            Blink();
            DOVirtual.DelayedCall(0.5f, () =>
            {
                BG.gameObject.SetActive(false);
                scoreViewBody.gameObject.SetActive(false);
                rankingButton.gameObject.SetActive(false);
                tweetButton  .gameObject.SetActive(false);
                restartButton.gameObject.SetActive(false);
                dead.gameObject.SetActive(false);
                gameObject.SetActive(false);
                
                ranking.Enter(scoreCounter.Score.Value);

                ranking.IsActive.Where(active => !active).Take(1)
                    .Subscribe(__ =>
                    {
                        BG.gameObject.SetActive(true);
                        scoreViewBody.gameObject.SetActive(true);
                        rankingButton.gameObject.SetActive(true);
                        tweetButton  .gameObject.SetActive(true);
                        restartButton.gameObject.SetActive(true);
                        dead.gameObject.SetActive(true);
                        dead.Play("dead_loop", 0, 0);
                        gameObject.SetActive(true);
                    });
            });
            
        });
    }

    [Button]
    void Tweet(string tweet)
    {
        var url = "https://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(tweet);
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenNewWindow(url);
#else
        Application.OpenURL(url);
#endif
    }
    
    public void Enter()
    {
        gameObject.SetActive(true);

        DOTween.Sequence()
            .AppendCallback(() =>
            {
                BG.gameObject.SetActive(true);
                dead.gameObject.SetActive(true);
                dead.enabled = true;
                dead.Play("dead_in", 0, 0);
                float delayLast = 0;
                deadSoundDelays.ForEach(delay =>
                {
                    delayLast += delay;
                    DOVirtual.DelayedCall(delayLast, () => audio.PlayOneShot(SoundDatabase.Instance.dead, 0.4f));
                });
            })
            .AppendInterval(4f)
            .AppendCallback(() =>
            {
                scoreViewBody.gameObject.SetActive(true);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                scoreViewBody.OnScoreSet(scoreCounter.Score.Value, 1.7f);
            })
            .AppendInterval(2f)
            .AppendCallback(() =>
            {
                rankingButton.gameObject.SetActive(true);
                tweetButton.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
            })
            .AppendInterval(0.4f)
            .AppendCallback(() =>
            {
                dead.Play("dead_loop");
            });
    }

    void OnRestart()
    {
        Blink();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            scoreViewBody.OnScoreSet(0);
            BG.gameObject.SetActive(false);
            scoreViewBody.gameObject.SetActive(false);
            rankingButton.gameObject.SetActive(false);
            tweetButton  .gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            dead.gameObject.SetActive(false);
            gameObject.SetActive(false);
            _Restart.OnNext(Unit.Default);
        });
    }

    void Blink()
    {
        blind.enabled = true;
        blind.Play("eyeEffect_UI_blink", 0, 0);
    }

    public IObservable<Unit> Restart => _Restart;
    
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void OpenNewWindow(string url);
#endif
}
