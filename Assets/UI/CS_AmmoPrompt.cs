/*
 *  To use this class, call SetMaxAmmo() to initialize the maximum amount of bullets a weapon has.
 *  Then call SubtractBullet() every time you want to update the UI to have one less bullet.
 *
 *  Made by: Xein
 */

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AmmoPrompt : MonoBehaviour
{
    [Header("Ammo")] 
    [SerializeField] private GameObject ammo;
    [SerializeField] private Sprite fullAmmo;
    [SerializeField] private Sprite emptyAmmo;

    private int _maxAmmo;
    private int _currentAmmo;
    private bool _ammoIsHidden;
    private RectTransform _transform;
    private List<Image> _ammoIcons;
    //private Animator _animator;
    //private static readonly int CloseAnim = Animator.StringToHash("CloseAnim");

    [Header("Inventory")] 
    [SerializeField] private GameObject[] slots;

    private void Awake()
    {
        ammo.SetActive(false);
        //_animator = ammo.GetComponent<Animator>();
        _transform = ammo.GetComponent<RectTransform>();
        // Set all disabled at the beginning
        _ammoIcons = ammo.GetComponentsInChildren<Image>().ToList();
        foreach (var spr in _ammoIcons.ToList())
        {
            if (spr.name != "B") _ammoIcons.Remove(spr);
            spr.gameObject.SetActive(false);
        }

        foreach (var s in slots)
        {
            var button = s.GetComponent<UIButton>();
            StartCoroutine(button.SetText("Empty"));
            button.ignoreMouse = true;
        }
        
        ChangeActiveSlot(0);
    }

    public void SetCurrentAmmo(int value)
    {
        for (var i = 0; i < _maxAmmo; i++)
        {
            _ammoIcons[i].sprite = emptyAmmo;
        }

        for (int i = 0; i < value; i++)
        {
            _ammoIcons[i].gameObject.SetActive(true);
            _ammoIcons[i].sprite = fullAmmo;
        }
        
        _currentAmmo = value;
    }

    public void SetSlot(int id, string name) => 
        StartCoroutine(slots[id].GetComponent<UIButton>().SetText(name));
    
    public void SetSlotEmpty(int id) =>
        StartCoroutine(slots[id].GetComponent<UIButton>().SetText("Empty"));

    public void ChangeActiveSlot(int id)
    {
        for (var i = 0; i < 2; i++)
        {
            if (i == id)
                slots[i].GetComponent<UIButton>().SetFocus();
            else
                slots[i].GetComponent<UIButton>().RemoveFocus();
        }
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
        ShowBullets();
        foreach (var icons in _ammoIcons)
        {
            icons.gameObject.SetActive(false);
            icons.sprite = emptyAmmo;
        }
        
        for (var i = 0; i < _maxAmmo; i++)
        {
            _ammoIcons[i].gameObject.SetActive(true);
            _ammoIcons[i].sprite = fullAmmo;
        }

        _currentAmmo = _maxAmmo;
    }

    public void DoHide()
    {
        if(_ammoIsHidden)
            return;
        
        HideBullets();
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
            
        /*if (_currentAmmo == 0)
        {
            //_ammoIcons[0].sprite = emptyAmmo;
            HideBullets();
            return;
        }*/
    }

    private void ShowBullets()
    {
        ammo.SetActive(true);
        _ammoIsHidden = false;
    }

    private void HideBullets()
    {
        //  _animator.SetTrigger(CloseAnim);
        ammo.SetActive(false);
        _ammoIsHidden = true;
    }
}
