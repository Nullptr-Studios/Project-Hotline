using System.Collections;
using System.Collections.Generic;
using CC.MessagingCentre;
using TMPro;
using UnityEngine;

public class ActPopup : MonoBehaviour
{
    [SerializeField] private string date;
    public float dateSpeed;
    [SerializeField] private string title;
    
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI titleText;
    
    void Start()
    {
        dateText.text = "";
        titleText.text = title;
    }

    private IEnumerator ShowDate()
    {
        for (var i = 0; i < date.Length; i++)
        {
            dateText.text = date.Substring(0, i+1);
            yield return new WaitForSeconds(dateSpeed/date.Length);
        }
    }
    
    private void StartShowDate() => StartCoroutine(ShowDate());

    private void StartDateSound()
    { }

    private void EndDateSound()
    { }

    private void TitleSound() 
    { }

    private void DestroySelf()
    {
        gameObject.SetActive(false);
        GameObject.Find("NovelManager").SendMessage("StartVNConversation", value: 0);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
