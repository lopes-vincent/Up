using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    [SerializeField] string apiKey;
    [SerializeField] private TextMeshProUGUI textTimer;

    [SerializeField] private Score playerScoreDisplay;
    [SerializeField] private GameObject playerScoreContainer;
    [SerializeField] private GameObject thanksContainer;
    [SerializeField] private GameObject scoreForm;
    [SerializeField] private Leaderboard leaderboard;
    
    private Timer _timer;
    private string _playerName;
    private List<Score> _scoreGameObjects;
    
    void Awake()
    {
        _timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
    }

    private void Start()
    {
        _timer.StopTimer();
        _timer.HideGameTimer();
        textTimer.text = _timer.getMsElapsedAsText();
        _scoreGameObjects = leaderboard.GetScoreGameObjects();
    }

    public void Restart()
    {
        _timer.Restart();
        SceneManager.LoadScene(1);
    }

    public void SetPlayerName(string name)
    {
        _playerName = name;
    }

    public void SubmitScore()
    {
        if (_playerName.Length > 1)
        {
            scoreForm.gameObject.SetActive(false);
            thanksContainer.SetActive(true);
            StartCoroutine(PostScore(_playerName, _timer.getMsElapsed()));
        }
    }
    
    IEnumerator PostScore(string name, float score)
    {
        SHA256 sha = SHA256.Create();
        PlayerScore playerScore = new PlayerScore();
        playerScore.uuid = Guid.NewGuid().ToString();
        playerScore.name = name;
        playerScore.SetScoreInSeconds(score);
        playerScore.hash = HashString(playerScore.name + playerScore.score);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(leaderboard.apiRoute+"/up", JsonUtility.ToJson(playerScore),  "application/json"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
                yield break;
            }

            yield return leaderboard.RefreshScores();
            
            playerScore = JsonUtility.FromJson<PlayerScore>(webRequest.downloadHandler.text);
            int playerScoreIndex = playerScore.position - 1;
            if (_scoreGameObjects.ElementAtOrDefault(playerScoreIndex))
            {
                _scoreGameObjects[playerScoreIndex].SetScore(Timer.sToText(playerScore.GetScoreInSeconds()));
                _scoreGameObjects[playerScoreIndex].SetCreatedAt(playerScore.createdAt);
                _scoreGameObjects[playerScoreIndex].SetName(playerScore.name);
                _scoreGameObjects[playerScoreIndex].SetPosition(playerScore.position.ToString());
                _scoreGameObjects[playerScoreIndex].gameObject.SetActive(true);
                _scoreGameObjects[playerScoreIndex].HighlightText();
            }
            else
            {
                playerScoreDisplay.SetScore(Timer.sToText(playerScore.GetScoreInSeconds()));
                playerScoreDisplay.SetCreatedAt(playerScore.createdAt);
                playerScoreDisplay.SetName(playerScore.name);
                playerScoreDisplay.SetPosition(playerScore.position.ToString());
                playerScoreDisplay.gameObject.SetActive(true);
                playerScoreContainer.SetActive(true);
            }
        }
    }
    
    public string HashString(string text)
    {
        if (String.IsNullOrEmpty(text))
        {
            return String.Empty;
        }
    
        using (var sha = new SHA256Managed())
        {
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + apiKey);
            byte[] hashBytes = sha.ComputeHash(textBytes);
        
            string hash = BitConverter
                .ToString(hashBytes)
                .Replace("-", String.Empty);

            return hash;
        }
    }
}