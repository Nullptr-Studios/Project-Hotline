using System.Collections;
using CC.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NovelUIController : BaseDialogueUIController
{
    [SerializeField] [Range(0.02f, 0.07f)] private float defaultTextSpeed = 0.04f;
    [SerializeField] [Range(0.001f, 0.02f)] private float fastTextSpeed = 0.01f;
    
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CS_NovelUICharacter speaker;
    // TODO: Character Sprite
    // TODO: Next Icon
    // TODO: Response System
    private Canvas _canvas;
    private PlayerIA _input;
    
    private bool _isShowing;
    private bool _isAnimatingText;
    private bool _handledInput;
    private float _textSpeed;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool logInput;
#endif
    
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        //_canvas.enabled = false;
        
        _textSpeed = defaultTextSpeed;
    }
    

    protected override IEnumerator showSentence(string speakerName, Sprite characterSprite,
        bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
    {
        _isAnimatingText = true;
        Show();
        
        text.text = _currentTextMod?.Sentence;
        text.maxVisibleCharacters = 0;
        speaker.SetName(speakerName);
        
        for (var i = 0; i < _currentTextMod?.Sentence.Length; i++)
        {
            yield return StartCoroutine(processTagsForPosition(i));

            text.maxVisibleCharacters++;
            yield return new WaitForSeconds(_textSpeed * _speedMultiplyer);
        }
        
        _isAnimatingText = false;
    }
    
    /// <summary>
    /// Logic for text speed and next text
    /// </summary>
    /// <param name="context"></param>
    private void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
#if UNITY_EDITOR
            if (logInput) Debug.Log("NovelUIController: Interact.Enable");
#endif
            
            // Uses fast speed if text is animating, pass to next dialogue if not
            if (_isAnimatingText)
                _textSpeed = fastTextSpeed;
            else
                DialogueController.Instance.Next();
        }
        else if (context.canceled)
        {
            
#if UNITY_EDITOR
            if (logInput) Debug.Log("NovelUIController: Interact.Cancel");
#endif
            
            _textSpeed = defaultTextSpeed;
        }
    }

    private void Show()
    {
        _canvas.enabled = true;
    }
    
    #region INPUT_SYSTEM
    private void OnEnable()
    {
        _input = new PlayerIA();
        _input.Gameplay.Interact.Enable();
        _input.Gameplay.Interact.performed += Interact;
        _input.Gameplay.Interact.canceled += Interact;
    }

    private void OnDisable()
    {
        _input.Gameplay.Interact.Disable();
    }
    #endregion
    
}
