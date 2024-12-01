using CC.DialogueSystem;
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
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject[] mooks;
    [SerializeField] private GameObject glitchVolume;
    [SerializeField] private Vector2 playerPos;
    [SerializeField] private Vector2 bossPos;
    [SerializeField] private float playerRot;
    [SerializeField] private float bossRot;

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
        SceneManager.LoadScene("MainMenu");
    } // TODO: This should call the loading screen

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
        if (player == null || boss == null) return;
        
        player.transform.position = new Vector3(playerPos.x, playerPos.y, player.transform.position.z);
        player.transform.rotation = Quaternion.Euler(0, 0, playerRot);
        boss.transform.position = new Vector3(bossPos.x, bossPos.y, boss.transform.position.z);
        boss.transform.rotation = Quaternion.Euler(0, 0, bossRot);
        boss.GetComponent<EnemyDamageable>().UpdateHealth(1);
    }

    public void JacobGlitch()
    {
        if (glitchVolume == null) return;
        glitchVolume.SetActive(true);
        var pixelCamera = GameObject.Find("Cinemachine Brain").GetComponent<PixelPerfectCamera>();
        if (pixelCamera != null) pixelCamera.enabled = false;
    }
}
