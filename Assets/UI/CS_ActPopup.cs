using System.Collections;
using System.Collections.Generic;
using CC.MessagingCentre;
using TMPro;
using UnityEngine;

public class ActPopup : MonoBehaviour
{
    [SerializeField] private string date;
    public float dateSpeed;
    public float titleSpeed;
    [SerializeField] private string title;
    
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private List<TextMeshProUGUI> titleText;
    [SerializeField] private bool openConversationOnClose;
    [SerializeField] private int conversationId;
    
    void Start()
    {
        dateText.text = "";
        titleText[0].text = "";
    }

    private IEnumerator ShowDate()
    {
        for (var i = 0; i < date.Length; i++)
        {
            dateText.text = date.Substring(0, i+1);
            yield return new WaitForSeconds(dateSpeed/date.Length);
        }
    }
    
    private IEnumerator ShowTitle(int Index)
    {
        for (var i = 0; i < title.Length; i++)
        {
            titleText[Index].text = title.Substring(0, i+1);
            yield return new WaitForSeconds(titleSpeed/title.Length);
        }
    }
    
    private void StartShowDate() => StartCoroutine(ShowDate());

    private void StartShowTitle()
    {
        for (int i = 0; i < titleText.Count; i++)
            StartCoroutine(ShowTitle(i));
    }

    private void StartDateSound()
    { }

    private void EndDateSound()
    { }

    private void TitleSound() 
    { }

    private void DestroySelf()
    {
        gameObject.SetActive(false);
        if (openConversationOnClose)
            GameObject.Find("NovelManager").SendMessage("StartVNConversation", value: conversationId);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        for (int k = 0; k < titleText.Count; k++)
        {
            //Update mesh
            titleText[k].ForceMeshUpdate();
            Mesh mesh = titleText[k].mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 offset = Wobble(Time.time + i);

                vertices[i] = vertices[i] + offset;
            }

            mesh.vertices = vertices;
            titleText[k].canvasRenderer.SetMesh(mesh);
            
            //Update rotation
            titleText[k].transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin((Time.time - k*.25f) * 2.5f) * 2.5f);
        }
    }
    
    Vector2 Wobble(float time) {
        return new Vector2(Mathf.Sin(time*3.3f) * .5f, Mathf.Cos(time*2.5f) * .5f);
    }
}
