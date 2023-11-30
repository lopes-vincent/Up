using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI position;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI createdAt;

    public void SetPosition(string positionValue)
    {
        position.text = positionValue;
    }
    
    public void SetName(string nameValue)
    {
        name.text = nameValue;
    }

    public void SetScore(string scoreValue)
    {
        score.text = scoreValue;
    }

    public void SetCreatedAt(string value)
    {
        createdAt.text = value;
    }

    public void HighlightText()
    {
        position.color = Color.green;
        name.color = Color.green;
        score.color = Color.green;
        createdAt.color = Color.green;
    }
}
