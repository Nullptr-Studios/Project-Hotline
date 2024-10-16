using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AmmoPrompt : MonoBehaviour
{
    [SerializeField] private Sprite fullAmmo;
    [SerializeField] private Sprite emptyAmmo;
    [SerializeField] private GameObject ammoPrefab;

    private int _maxAmmo;
    private int _currentAmmo;
    private List<Image> _ammoIcons;

    private void Awake()
    {
        // Set all diabled at the beginning
        _ammoIcons = gameObject.GetComponentsInChildren<Image>().ToList();
        foreach (var spr in _ammoIcons)
        {
            spr.gameObject.SetActive(false);
        }
    }

    public void SetMaxAmmo(int value) 
    { 
        _maxAmmo = value;
        for (int i = 0; i < _maxAmmo; i++)
        {
            _ammoIcons[i].gameObject.SetActive(true);
            _ammoIcons[i].sprite = fullAmmo;
        }

        _currentAmmo = _maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
