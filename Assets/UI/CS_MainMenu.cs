using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI version;
    
    // Start is called before the first frame update
    void Start()
    {
        version.text = Application.version;
    }

    public void OnTutorial() => SceneManager.LoadScene("Tutorial");
    
    public void OnDemo() => SceneManager.LoadScene("DemoScene");
    
    public void OnExit() => Application.Quit();
}
