using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTex;
    [SerializeField] private GameObject scoreGameObject;
    [SerializeField] private ColorTemplate colorTemplate;

    private int score;

    public int Score
    {
        set
        {
            if (value <= 0)
            {
                score = 0;
                return;
            }

            score = NodeValue.ClosestTwoPower(value);
            scoreGameObject.GetComponent<Image>().color = colorTemplate.GetColorForValue(score);
        }
    }


    private void Update()
    {
        if (score > 0)
        {
            scoreTex.text = score.ToString();
            scoreGameObject.SetActive(true);
            return;
        }

        scoreGameObject.SetActive(false);
    }
}
