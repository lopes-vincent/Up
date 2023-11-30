using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<Score> _scoreGameObjects;

    public string apiRoute;
    
    private void Start()
    {
        StartCoroutine(RefreshScores());
    }

    public  List<Score> GetScoreGameObjects()
    {
        return _scoreGameObjects;
    }

    public IEnumerator RefreshScores()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiRoute))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                yield break;
            }
            
            PlayerScoreCollection playerScores = JsonUtility.FromJson<PlayerScoreCollection>("{\"scores\":" +webRequest.downloadHandler.text + "}");

            int index = 0;
            foreach (PlayerScore playerScore in playerScores.scores)
            {
                if (_scoreGameObjects.ElementAtOrDefault(index))
                {
                    _scoreGameObjects[index].SetScore(Timer.msToText(playerScore.score));
                    _scoreGameObjects[index].SetCreatedAt(playerScore.createdAt);
                    _scoreGameObjects[index].SetName(playerScore.name);
                    _scoreGameObjects[index].SetPosition((index + 1).ToString());
                    _scoreGameObjects[index].gameObject.SetActive(true);
                }
                index++;
            }
        }
    }    
}

[Serializable]
public class PlayerScoreCollection
{
    public PlayerScore[] scores;
}


[Serializable]
public class PlayerScore
{
    public string uuid;
    public string name;
    public int score;
    public string hash;
    public int position;
    public string createdAt;
}

