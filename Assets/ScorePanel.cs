using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTex;
    [SerializeField] private GameObject scoreGameObject;

    private int score;

    public void SetScore(int score)
    {
        this.score = score;
    }

    private void Update()
    {
        if(score > 0)
        {
            scoreTex.text = score.ToString();
            scoreGameObject.SetActive(true);
            return;
        }

        scoreGameObject.SetActive(false);
    }
}
