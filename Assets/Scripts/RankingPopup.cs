using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RankingPopup : MonoBehaviour
{
    [SerializeField] RankUnit rankUnitPrefab;

    [SerializeField] ScoreViewSimple yourScore;
    [SerializeField] ScoreViewSimple highScore;
    [SerializeField] Transform rankContentsParent;
    [SerializeField] InputField inputField;
    [SerializeField] Button sendButton;
    [SerializeField] Button exitButton;

    [SerializeField] Animator blind;

    [SerializeField] int lastScore = -1;
    [SerializeField] int bestScore = -1;
    
    [SerializeField, ReadOnly] BoolReactiveProperty _IsActive = new BoolReactiveProperty(false);
    public IReadOnlyReactiveProperty<bool> IsActive => _IsActive;

    List<RankUnit> rankUnits;
    RankUnit myUnit;

    bool BestScore()
    {
        return lastScore > bestScore;
    }

    [Button]
    public void Enter(int score = -1)
    {
        foreach (Transform unit in rankContentsParent)
        {
            Destroy(unit.gameObject);
        }
        rankUnits = null;
        myUnit = null;
        
        lastScore = score;
        if(score != -1) yourScore.Set(score);
        
        GetRanking();
        GetYourHighScore();
        
        gameObject.SetActive(true);
        _IsActive.Value = true;
    }

    void GetRanking()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Login(() => GetRanking());
            return;
        }
        
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            StatisticName = "totalScore",
            StartPosition = 0,
            MaxResultsCount = 100
        };
        PlayFabClientAPI.GetLeaderboard
        (
            request,
            result =>
            {
                rankUnits = new List<RankUnit>();
                result.Leaderboard.ForEach(entry =>
                {
                    var unit = Instantiate(rankUnitPrefab, rankContentsParent);
                    unit.Init(entry.Position + 1, entry.DisplayName, entry.StatValue);
                    rankUnits.Add(unit);
                    if (entry.PlayFabId == myID) myUnit = unit;
                });
            },
            error  => print(error.GenerateErrorReport())
        );
    }

    void GetYourHighScore()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Login(() => GetYourHighScore());
            return;
        }
        
        GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest()
        {
            StatisticNames = new List<string>() {"totalScore"}
        };
        PlayFabClientAPI.GetPlayerStatistics
        (
            request,
            result =>
            {
                if (result.Statistics is null) return;

                var data = result.Statistics
                    .FirstOrDefault(stat => stat.StatisticName == "totalScore");
                if(data is null) return;

                bestScore = data.Value;
                highScore.Set(data.Value);
            },
            error => print(error.GenerateErrorReport())
        );
    }
    

    void Awake()
    {
        sendButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(!BestScore()) return;
            
            SendName(inputField.text);
            SendScore(lastScore, inputField.text);
        });
        exitButton.OnClickAsObservable().Subscribe(_ =>
        {
            blind.enabled = true;
            blind.Play("eyeEffect_UI_blink", 0, 0);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                _IsActive.Value = false;
                gameObject.SetActive(false);
            });
        });
    }

    void Update()
    {
        sendButton.interactable = IsValidName(inputField.text) && BestScore();
    }

    void SendName(string name)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Login(() => SendName(name));
            return;
        }
        
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName
        (
            request,
            result => { },
            error  => print(error.GenerateErrorReport())
        );
    }

    void SendScore(int score, string name)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Login(() => SendScore(score, name));
            return;
        }
        
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "totalScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics
        (
            request,
            result =>
            {
                var below = rankUnits.FirstOrDefault(unit => unit.Score <= score);
                int index = (below is null) ? rankUnits.Count : below.transform.GetSiblingIndex();
                if (myUnit is null)
                {
                    myUnit = Instantiate(rankUnitPrefab, rankContentsParent);
                    myUnit.Init(index + 1, name, score);
                    rankUnits.Skip(index).ForEach(unit => unit.Rank++);
                }
                else
                {
                    rankUnits.Remove(myUnit);

                    int oldRank = myUnit.Rank;
                    
                    myUnit.Rank = index + 1;
                    myUnit.Name = name;
                    myUnit.Score = score;

                    rankUnits.Take(oldRank - 1).Skip(index).ForEach(unit => unit.Rank++);
                }
                myUnit.transform.SetSiblingIndex(index);
                rankUnits.Insert(index, myUnit);

                bestScore = score;
                highScore.Set(bestScore);
            },
            error  => print(error.GenerateErrorReport())
        );
    }

    bool IsValidName(string name)
    {
        if (name.Length < 3)  return false;
        if (name.Length > 10) return false;
        
        return true;
    }


    [SerializeField] string loginID;
    string myID;
    void Login(Action onEnd)
    {
        var request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = loginID == "" ? SystemInfo.deviceUniqueIdentifier : loginID //u1w程度なら？
        };
        PlayFabClientAPI.LoginWithCustomID
        (
            request,
            result =>
            {
                myID = result.PlayFabId;
                onEnd.Invoke();
            },
            error  => print(error.GenerateErrorReport())
        );
    }
}