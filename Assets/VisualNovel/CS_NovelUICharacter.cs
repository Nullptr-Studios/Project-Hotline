using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CS_NovelUICharacter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;
    [SerializeField] private Image border;

    public bool IsVisible { get; private set; }

    /// <summary>
    /// Set speaking character name
    /// </summary>
    /// <param name="name">Name</param>
    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Hide();
            return;
        }
        
        label.text = name;
        Show();
        
    } 

    private void Hide()
    {
        IsVisible = false;
        background.enabled = false;
        border.enabled = false;
        label.text = string.Empty;
    }

    private void Show()
    {
        IsVisible = true;
        background.enabled = true;
        border.enabled = true;
    }
}
