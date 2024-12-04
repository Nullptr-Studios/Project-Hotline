using System.Collections;
using CC.DialogueSystem;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BoxCollider2D missionTrigger;
    [SerializeField] private BoxCollider2D endTrigger;
    [SerializeField] [CanBeNull] private MissionObjective objective;
    [SerializeField] private ScoreUI score;
    [CanBeNull] [SerializeField] private GameObject actPopup;
    
    [Header("Bossfight settings")]
    [SerializeField] private GameObject glitchVolume;
    [SerializeField] private GameObject killScreen;
    [SerializeField] private GameObject blackScreen;
    private int _mercyKills;
    public EventReference shotShound;
    public EventReference doorShound;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool logMissionEnd;
#endif
    
    // Start is called before the first frame update
    void Start()
    {
        _mercyKills = 0;
        
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
        //DONE: Add Mercy check!!!!!!!!!!
        // Maybe this isnt necesary as the next scene to mercy should be the credit scene -x
        // this is literally the worst coding practice in existance -x
        var nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextScene == 13)
        {
            SceneManager.LoadScene("MainMenu2");
            return;
        }
        
        SceneManager.LoadScene(nextScene);
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
        if (glitchVolume != null) glitchVolume.SetActive(false);
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

    public void SantoroFight()
    {
        if (killScreen != null) killScreen.SetActive(true);
        /*FMODUnity.*/RuntimeManager.PlayOneShot(shotShound);
        StartCoroutine(Wait(0.5f));
        if (killScreen != null) killScreen.SetActive(false);
        OpenScore();
        return;

        IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }

    public void JacobGlitch()
    {
        if (glitchVolume == null) return;
        glitchVolume.SetActive(true);
        var pixelCamera = GameObject.Find("Cinemachine Brain").GetComponent<PixelPerfectCamera>();
        if (pixelCamera != null) pixelCamera.enabled = false;
    }

    public void BlakeDoor()
    {
        /*FMODUnity.*/RuntimeManager.PlayOneShot(doorShound);
    }

    public void BlakeShot()
    {
        if (blackScreen != null) blackScreen.SetActive(true);
        /*FMODUnity.*/RuntimeManager.PlayOneShot(shotShound);
        StartCoroutine(Wait(0.5f));
        if (blackScreen != null) blackScreen.SetActive(false);
        LoadNextScene();
        return;

        IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }

    public void KillScreen()
    {
        killScreen.SetActive(true);
    }

    public void EndMercy()
    {
        _mercyKills++;
        if (_mercyKills >= 4) OpenScore();
    }
    
    public void StartGlitches() => glitchVolume.SetActive(true);
    public void EndGlitches() => glitchVolume.SetActive(false);
}
