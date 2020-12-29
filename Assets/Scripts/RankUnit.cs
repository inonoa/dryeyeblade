using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class RankUnit : MonoBehaviour
{
    [SerializeField] Text rankText;
    [SerializeField] Text nameText;
    [SerializeField] Text scoreText;

    int rank;
    public int Rank
    {
        get => rank;
        set
        {
            rank = value;
            rankText.text = rank.ToString();
        }
    }

    new string name;
    public string Name
    {
        get => name;
        set
        {
            name = value;
            nameText.text = name;
        }
    }

    int score;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }
    
    public void Init(int rank, string name, int score)
    {
        (Rank, Name, Score) = (rank, name, score);
        
        ChangeFontFilterMode();
    }

    [Button]
    void ChangeFontFilterMode()
    {
        foreach (var text in new[]{ rankText, nameText, scoreText })
        {
            text.font.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
}