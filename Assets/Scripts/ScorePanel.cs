using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTex;
    [SerializeField] private NodeValue nodeValue;
    [SerializeField] private ColorTemplate colorTemplate;
    [SerializeField] private Image circleInImage;

    private void Start()
    {
        nodeValue.gameObject.SetActive(false);
    }

    public int Score
    {
        set
        {
            nodeValue.Value = value;
            nodeValue.gameObject.SetActive(nodeValue.Value > 0);
        }
    }
    
}
