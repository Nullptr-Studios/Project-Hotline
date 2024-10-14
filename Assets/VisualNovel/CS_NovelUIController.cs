using System.Collections;
using CC.DialogueSystem;
using TMPro;
using UnityEngine;

public class NovelUIController : BaseDialogueUIController
{
    [SerializeField] private float defaultTextSpeed;
    [SerializeField] private float fastTextSpeed;
    
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CS_NovelUICharacter speaker;
    private Canvas _canvas;
    
    private bool _isShowing;
    private bool _isAnimatingText;
    private bool _handledInput;
    private float _textSpeed;
    
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        
        _textSpeed = defaultTextSpeed;
    }
    
    void Update()
    {
        
    }

    protected override IEnumerator showSentence(string speakerName, Sprite characterSprite,
        bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
    {
        _isAnimatingText = true;
        Show();
        
        text.text = _currentTextMod?.Sentence;
        speaker.SetName(speakerName);
        
        yield return new WaitForSeconds(_textSpeed * _speedMultiplyer);
    }

    private void Show()
    {
        _canvas.enabled = true;
    }
}
