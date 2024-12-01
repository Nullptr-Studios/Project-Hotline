using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    private Score _score;
    private PlayerIA _input;

    [SerializeField] private float killsTime = 1;
    [SerializeField] private float deathsTime = 1;
    [SerializeField] private float timeTime = 0.8f;
    [SerializeField] private float scoreTime = 2.5f;
    [SerializeField] private bool goToConversation = false;
    [SerializeField] private int conversationID;
    
    [Header("Components")]
    [SerializeField] private GameObject scoreObject;
    [SerializeField] private TextMeshProUGUI scoreValue;
    [SerializeField] private GameObject killsObject;
    [SerializeField] private TextMeshProUGUI killsValue;
    [SerializeField] private GameObject deathsObject;
    [SerializeField] private TextMeshProUGUI deathsValue;
    [SerializeField] private GameObject timeObject;
    [SerializeField] private TextMeshProUGUI timeValue;
    [SerializeField] private GameObject exitScreen;

    private void Awake()
    {
        _input = new PlayerIA();
        _input.UI.Accept.performed += ExitScreen;
        _input.Gameplay.Interact.performed += ChangeSpeed;
        _input.Gameplay.Interact.canceled += ChangeSpeed;
    }

    private void ChangeSpeed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) // change to started if this gives problems -x
        {
            scoreTime *= 0.5f;
            killsTime *= 0.5f;
            deathsTime *= 0.5f;
            timeTime *= 0.5f;
        }
        else if (ctx.canceled)
        {
            scoreTime *= 2;
            killsTime *= 2;
            deathsTime *= 2;
            timeTime *= 2;
        }
    }

    private void OnEnable()
    {
        _score = ScoreManager.CalculateScore();
        scoreObject.SetActive(false);
        killsObject.SetActive(false);
        deathsObject.SetActive(false);
        timeObject.SetActive(false);
        
        _input.Gameplay.Interact.Enable();

        StartCoroutine(ShowText());

        // scoreText.text = _score.Value.ToString();
        // killsText.text = _score.Kills.ToString();
        // deathText.text = _score.Deaths.ToString();
        // int minutes = (int)Mathf.Floor(_score.Time / 60);
        // int seconds = (int)_score.Time % 60;
        // timeText.text = $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Shows characters with an animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowText()
    {
        // Show kills
        int kills = _score.Kills;
        killsObject.SetActive(true);
        for (int i = 0; i < kills; i++)
        {
            killsValue.text = (i+1).ToString();
            yield return new WaitForSeconds(killsTime/kills);
        }
        
        if (kills == 0)
            yield return new WaitForSeconds(deathsTime);
        
        int deaths = _score.Deaths;
        deathsObject.SetActive(true);
        for (int i = 0; i < deaths; i++)
        {
            deathsValue.text = (i+1).ToString();
            yield return new WaitForSeconds(deathsTime/deaths);
        }

        if (deaths == 0)
            yield return new WaitForSeconds(deathsTime);

        int minutes = (int)Mathf.Floor(_score.Time / 60);
        int seconds = (int)_score.Time % 60;
        timeObject.SetActive(true);
        timeValue.text = $"{minutes:00}:{seconds:00}";
        yield return new WaitForSeconds(timeTime);
        
        int score = (int)_score.Value;
        scoreObject.SetActive(true);
        for (int i = 0; i < score; i+=9)
        {
            scoreValue.text = (i+1).ToString();
            yield return new WaitForSeconds(scoreTime/score);
        }
        scoreValue.text = ((int)_score.Value).ToString();
        
        yield return new WaitForSeconds(2);
        
        EnableExit();
    }

    /// <summary>
    /// Open press x to continue 
    /// </summary>
    private void EnableExit()
    {
        exitScreen.SetActive(true);
        _input.Gameplay.Interact.Disable();
        _input.UI.Accept.Enable();
    }

    /// <summary>
    /// Go to next scene logic
    /// </summary>
    /// <param name="context"></param>
    private void ExitScreen(InputAction.CallbackContext context)
    {
        _input.UI.Accept.performed -= ExitScreen;
        _input.Gameplay.Interact.performed -= ChangeSpeed;
        _input.Gameplay.Interact.canceled -= ChangeSpeed;
        if (goToConversation)
        {
            GameObject.Find("NovelManager").GetComponent<ConversationHandler>().StartVNConversation(conversationID);
            return;
        }
        GameObject.Find("PA_LevelManager").SendMessage("EndLevelMessage");
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
