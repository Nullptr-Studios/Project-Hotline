/*
 *  To use this class, call SetMaxAmmo() to initialize the maximum amount of bullets a weapon has.
 *  Then call SubtractBullet() every time you want to update the UI to have one less bullet.
 *
 *  Made by: Xein
 */

using System.Collections.Generic;
using System.Linq; // .ToList() function
using UnityEngine;
using UnityEngine.UI;

public class AmmoPrompt : MonoBehaviour
{
    [SerializeField] private Sprite fullAmmo;
    [SerializeField] private Sprite emptyAmmo;
    [SerializeField] [Range(2f, 8f)] private float timeToHide = 3f;

    private int _maxAmmo;
    private int _currentAmmo;
    private float _timer;
    private bool _isHidden;
    private RectTransform _transform;
    private List<Image> _ammoIcons;
    private Animator _animator;
    private static readonly int CloseAnim = Animator.StringToHash("CloseAnim");

    private void Awake()
    {
        gameObject.SetActive(false);
        _animator = GetComponent<Animator>();
        _transform = GetComponent<RectTransform>();
        // Set all disabled at the beginning
        _ammoIcons = gameObject.GetComponentsInChildren<Image>().ToList();
        foreach (var spr in _ammoIcons)
        {
            spr.gameObject.SetActive(false);
        }
        
        // Idk why but unity picks self in GetComponentsInChildren sometimes, so I just remove if that's the case -x
        // Fucking Unity -x
        if (_ammoIcons[0].name == gameObject.name) _ammoIcons.RemoveAt(0);
    }

    private void Update()
    {
        //tf is this, this makes the ui hidden, but the player could still be with a weapon!!!!!!!!!!
        /*if (Time.time - _timer > timeToHide && !_isHidden)
        {
            Hide();
        }*/
    }

    public void SetCurrentAmmo(int value)
    {
        for (var i = 0; i < _maxAmmo; i++)
        {
            _ammoIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < value; i++)
        {
            _ammoIcons[i].gameObject.SetActive(true);
            _ammoIcons[i].sprite = fullAmmo;
        }
        
        _currentAmmo = value;
    }

    /// <summary>
    /// Sets the maximum amount of bullets on the file
    /// </summary>
    /// <param name="value">Max ammunition value</param>
    /// <param name="rowSize">Number of bullets per row</param>
    public void SetMaxAmmo(int value, int rowSize) 
    {
        _maxAmmo = value;
        _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rowSize * 4 + 3);
        Show();
        for (var i = 0; i < _maxAmmo; i++)
        {
            _ammoIcons[i].gameObject.SetActive(true);
            _ammoIcons[i].sprite = fullAmmo;
        }

        _currentAmmo = _maxAmmo;
    }

    public void DoHide()
    {
        if(_isHidden)
            return;
        
        Hide();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SubtractBullet()
    {
        _currentAmmo--;

        for (var i = _currentAmmo; i < _maxAmmo; i++)
        {
            //Safe check
            if (i < 0)
                break;
            
            if (_ammoIcons[i].sprite == fullAmmo)
                _ammoIcons[i].sprite = emptyAmmo;
        }
            
        if (_currentAmmo == 0)
        {
            //_ammoIcons[0].sprite = emptyAmmo;
            Hide();
            return;
        }
        
        _timer = Time.time;
    }

    private void Show()
    {
        gameObject.SetActive(true);
        _timer = Time.time;
        _isHidden = false;
    }

    private void Hide()
    {
        _animator.SetTrigger(CloseAnim);
        _isHidden = true;
    }
}
