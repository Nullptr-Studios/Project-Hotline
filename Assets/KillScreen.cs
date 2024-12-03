using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillScreen : MonoBehaviour
{
    public TextMeshProUGUI text;
    private string[] words = { "KILL", "KILL\nTHEM", "KILL\nTHEM\nALL" };
    private float timer;

    // Start is called before the first frame update
    void Awake()
    {
        timer = Time.time;
        text.text = "";
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time - timer > 2.5)
            gameObject.SetActive(false);
        else if (Time.time - timer > 1.2)
            text.text = words[2];
        else if (Time.time - timer > 0.7)
            text.text = words[1];
        else if (Time.time - timer > 0.3)
            text.text = words[0];
    }
}
