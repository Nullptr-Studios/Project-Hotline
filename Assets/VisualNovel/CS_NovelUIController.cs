using System.Collections;
using System.Collections.Generic;
using CC.DialogueSystem;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NovelUIController : BaseDialogueUIController
{
    [SerializeField] [Range(0.02f, 0.07f)] private float defaultTextSpeed = 0.04f;
    [SerializeField] [Range(0.001f, 0.02f)] private float fastTextSpeed = 0.001f;
    [SerializeField] private GameObject optionsPrefab;
    
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private NovelUICharacter speaker;
    [SerializeField] private NovelUISprite spriteBlake;
    [SerializeField] private NovelUISprite spriteOther;
    [SerializeField] private Image continueButton;
    private NovelOptionsController _optionsController;
    [CanBeNull] private PlayerMovement _player;
    private Canvas _canvas;
    private PlayerIA _input;
    private Animator _animator;
    private static readonly int Blake = Animator.StringToHash("Blake");
    private static readonly int Other = Animator.StringToHash("Delta");
    
    private bool _isShowing;
    private bool _isAnimatingText;
    private bool _handledInput;
    private float _textSpeed;

    private string _lastSpeaker = "";

    public delegate void StartGame();
    public static StartGame OnStartGame;

    public int timesItRestarts = 0;
    private int _restarts = 0;

    private bool cac = false;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool logInput;
#endif
    
    private void Awake()
    {
        cac = false;
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _animator = GetComponent<Animator>();
        
        if (GameObject.FindWithTag("Player") != null)
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        _input = new PlayerIA();
        
        _textSpeed = defaultTextSpeed;
        continueButton.gameObject.SetActive(false);
        
        //activate.SetActive(false);
        
        //LoadingScreen.OnFinalizedLoading += () => activate.SetActive(true);
        
    }
    

    protected override IEnumerator showSentence(string speakerName, Sprite characterSprite,
        bool sameSpeakerAsLastDialogue = true, bool autoProceed = false)
    {
        _isAnimatingText = true;
        continueButton.gameObject.SetActive(false);
        Show();
        
        text.text = _currentTextMod?.Sentence;
        text.ForceMeshUpdate();
        var length = text.GetParsedText().Length;
        text.maxVisibleCharacters = 0;

        //I need to fucking do this cuz the fucking parser returns false some times, event tought is the same, fuck json -D
        bool sameSpeakerAsLastDialogeMine = true;
        if (speakerName != _lastSpeaker)
        {
            _lastSpeaker = speakerName;
            sameSpeakerAsLastDialogeMine = false;
        }

        
        if (!sameSpeakerAsLastDialogeMine)
        {
            speaker.SetName(speakerName);
            if (speakerName == "Blake")
            {
                spriteBlake.SetSprite(characterSprite, speakerName);
                _animator.SetTrigger(Blake);
            }
            else
            {
                spriteOther.SetSprite(characterSprite, speakerName);
                _animator.SetTrigger(Other);
            }
        }
        
        for (var i = 0; i < length; i++)
        {
            yield return StartCoroutine(processTagsForPosition(i));

            if (Time.deltaTime > _textSpeed)
                text.maxVisibleCharacters += 2; 
            else 
                text.maxVisibleCharacters++;
            yield return new WaitForSeconds(_textSpeed * _speedMultiplyer);
        }
        
        _isAnimatingText = false;
        continueButton.gameObject.SetActive(true);
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

    private void OnSkipConversation(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) {
            Close();
        } 
        if (ctx.started)
        {
            Debug.Log(ctx.duration);
        } 
        if (ctx.canceled)
        {
            Debug.Log("Canceled");
        }
    }
   

    /// <summary>
    /// Logic for clicked option button
    /// </summary>
    /// <param name="index">Button index</param>
    public override void OptionButtonClicked(int index)
    {
        DialogueController.Instance.OptionSelected(index);
        EnableInput();
    }

    private void Show()
    {
        //@TODO: Add animation
        _canvas.enabled = true;
        EnableInput();
    }

    public override void Close()
    {
        //@TODO: Add animation
        _canvas.enabled = false;
        DisableInput();

        if (_restarts == timesItRestarts & !cac)
        {
            OnStartGame?.Invoke();
            cac = true;
        }
        else
            _restarts++;

    }

    #region INPUT_SYSTEM
    private void EnableInput()
    {
        _input.UI.Accept.Enable();
        _input.UI.Accept.performed += Interact;
        _input.UI.Accept.canceled += Interact;
        _input.UI.SkipConversation.Enable();
        _input.UI.SkipConversation.performed += OnSkipConversation;
        _input.UI.SkipConversation.started += OnSkipConversation;
        _input.UI.SkipConversation.canceled += OnSkipConversation;
        
        if (_player != null)
            _player.OnDisable();
    }

    private void DisableInput()
    {
        _input.UI.Accept.Disable();
        _input.UI.SkipConversation.Disable();
        
        if (_player != null)
            _player.OnEnable();
    }

    #endregion
    
}
