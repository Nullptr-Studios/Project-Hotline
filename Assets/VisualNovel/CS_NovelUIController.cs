using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NovelUIController : BaseDialogueUIController
{
    [SerializeField] [Range(0.02f, 0.07f)] private float defaultTextSpeed = 0.04f;
    [SerializeField] [Range(0.001f, 0.02f)] private float fastTextSpeed = 0.01f;
    [SerializeField] private GameObject optionsPrefab;
    
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private NovelUICharacter speaker;
    [SerializeField] private NovelUISprite sprite;
    [SerializeField] private Image continueButton;
    private NovelOptionsController _optionsController;
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
        continueButton.enabled = false;
    }
    

    protected override IEnumerator showSentence(string speakerName, Sprite characterSprite,
        bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
    {
        _isAnimatingText = true;
        continueButton.enabled = false;
        Show();
        
        text.text = _currentTextMod?.Sentence;
        text.maxVisibleCharacters = 0;
        
        if (!sameSpeakerAsLastDialogue)
        {
            speaker.SetName(speakerName);
            sprite.SetSprite(characterSprite, speakerName);
        }
        
        for (var i = 0; i < _currentTextMod?.Sentence.Length; i++)
        {
            yield return StartCoroutine(processTagsForPosition(i));

            text.maxVisibleCharacters++;
            yield return new WaitForSeconds(_textSpeed * _speedMultiplyer);
        }
        
        _isAnimatingText = false;
        continueButton.enabled = true;
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
    
    public override void ShowOptions(List<Option> options)
    {
        var optionsObject = Instantiate(optionsPrefab, _canvas.transform);
        _optionsController = optionsObject.GetComponent<NovelOptionsController>();
        DisableInput();
        
        StartCoroutine(_optionsController.ShowOptions(options));
    }
    
    /// <summary>
    /// Logic for clicked option button
    /// </summary>
    /// <param name="index">Button index</param>
    public override void OptionButtonClicked(int index)
    {
        DialogueController.Instance.OptionSelected(index);
        OnEnable();
    }

    private void Show()
    {
        _canvas.enabled = true;
    }

    public override void Close()
    {
        _canvas.enabled = false;
    }

    #region INPUT_SYSTEM
    private void OnEnable()
    {
        _input = new PlayerIA();
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void EnableInput()
    {
        _input.Gameplay.Interact.Enable();
        _input.Gameplay.Interact.performed += Interact;
        _input.Gameplay.Interact.canceled += Interact;
    }

    private void DisableInput()
    {
        _input.Gameplay.Interact.Disable();
    }

    #endregion
    
}