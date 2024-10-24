using TMPro;
using UnityEngine;

public class ScorePrinter : MonoBehaviour
{
    private string _scoreText;
    private ScoreManager _scoreManager;
    public void Activate()
    {
        _scoreManager.CalcScore();
        TMP_Text tmpText = GetComponent<TMP_Text>();
        _scoreText = ScoreManager.Score.ToString();
        tmpText.text = "Score:" + _scoreText;
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
