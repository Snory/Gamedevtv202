using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _score;
    public void OnScoreAdded(int i)
    {
        _score.text = "Score: " + i.ToString(); 
    }
}
