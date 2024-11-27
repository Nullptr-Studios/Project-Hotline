using UnityEngine;
using UnityEngine.UI;

public class NovelUISprite : MonoBehaviour
{
    // [SerializeField] private Vector2 blakePosition;
    // [SerializeField] private Vector2 otherPosition;
    
    [Header("Components")]
    [System.Obsolete("There is no background now")]
    [SerializeField] private Image background;
    [SerializeField] private Image character;

    public void SetSprite(Sprite sprite, string characterName)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"[NovelUISprite] {this.name}: There is no sprite, using default.");
            return;
        }
        
        character.sprite = sprite;
        
        // if (characterName == "Blake")
        //     GetComponent<RectTransform>().anchoredPosition = blakePosition;
        // else
        //     GetComponent<RectTransform>().anchoredPosition = otherPosition;
        
        // if (!_isActive) Show();
    }

    [System.Obsolete("There is no need for hiding/showing sprites now")]
    private void Show()
    {
        // background.enabled = true;
        character.enabled = true;
        
        // _isActive = true;
    }

    [System.Obsolete("There is no need for hiding/showing sprites now")]
    private void Hide()
    {
        // background.enabled = false;
        character.enabled = false;
        
        // _isActive = false;
    }
}
