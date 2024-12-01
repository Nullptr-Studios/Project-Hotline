using CC.DialogueSystem;
using JetBrains.Annotations;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D missionTrigger;
    [SerializeField] private BoxCollider2D endTrigger;
    [SerializeField] [CanBeNull] private MissionObjective objective;
    [SerializeField] private ScoreUI score;
    [CanBeNull] [SerializeField] private GameObject actPopup;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool logMissionEnd;
#endif
    
    // Start is called before the first frame update
    void Start()
    {
        // Log errors
        if (missionTrigger == null || endTrigger == null)
        {
            Debug.LogError($"[LevelManager] {name}: Error with colliders. Level unwinnable.");
            this.enabled = false;
        }
        
        if (objective == null)
        {
            Debug.LogError($"[LevelManager] {name}: Objective not found. Level unwinnable.");
            this.enabled = false;
        }
        else
        {
            missionTrigger.GetComponent<MissionTrigger>().Objective = objective.gameObject;
        }
        
    }

    public void CompleteMission()
    {
        endTrigger.enabled = true;
        missionTrigger.enabled = false;

#if UNITY_EDITOR
        if (logMissionEnd) Debug.Log($"[LevelManager] {name}: Mission completed");
#endif
        
    }

    public void PauseMission()
    {
        endTrigger.enabled = false;

#if UNITY_EDITOR
        if (logMissionEnd) Debug.Log($"[LevelManager] {name}: Mission paused");
#endif
        
    }

    /// <summary>
    /// Exit to main menu
    /// </summary>
    public void EndLevelMessage() {
        VariableRepo.Instance.RemoveAll();
        Destroy(GetComponent<ScoreManager>());
        
        GameObject.Find("ScreenLevelTransition").GetComponent<Animator>().SetTrigger("In");
        
        //Invoke(nameof(LoadNextScene), 1f);
        LoadNextScene();
        
        //SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextScene()
    {
        //TODO: Add Mercy check!!!!!!!!!!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Make the player restart the current level
    /// </summary>
    public static void RestartLevel()
    {
        VariableRepo.Instance.RemoveAll();

        // TODO: this should be fixed after prototype to not have the player read the VN every time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenScore()
    {
        //disable player
        GameObject.FindGameObjectWithTag("Player").SetActive(false);
        
#if UNITY_EDITOR
        if (logMissionEnd) Debug.Log($"[LevelManager] {name}: Opening Score...");
#endif

        score.Activate();
    }
    
    public void OpenActPopup()
    {
        if (actPopup != null) actPopup.SetActive(true);
        else Debug.LogError($"[LevelManager] {name}: Trying to open act popup but it doesn't exist.");
    } 
}
