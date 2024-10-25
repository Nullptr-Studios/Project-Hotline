using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NovelUISprite : MonoBehaviour
{
    [SerializeField] private Vector2 blakePosition;
    [SerializeField] private Vector2 otherPosition;
    
    [Header("Components")]
    [SerializeField] private Image background;
    [SerializeField] private Image character;
    
    private bool _isActive;

    public void SetSprite(Sprite sprite, string characterName)
    {
        if (sprite == null)
        {
            Hide();
            return;
        }
        
        character.sprite = sprite;
        
        if (characterName == "Blake")
            GetComponent<RectTransform>().anchoredPosition = blakePosition;
        else
            GetComponent<RectTransform>().anchoredPosition = otherPosition;
        
        if (!_isActive) Show();
    }

    private void Show()
    {
        background.enabled = true;
        character.enabled = true;
        
        _isActive = true;
    }

    private void Hide()
    {
        background.enabled = false;
        character.enabled = false;
        
        _isActive = false;
    }
}
