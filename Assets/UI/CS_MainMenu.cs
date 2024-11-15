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

    public void OnTutorial() => SceneManager.LoadScene("Tutorial__Main");
    
    public void OnLevel(string name) => SceneManager.LoadScene(name);
    
    public void OnExit() => Application.Quit();
}
