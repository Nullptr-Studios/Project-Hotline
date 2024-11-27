using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextEffect : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> Scoretext;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int k = 0; k < Scoretext.Count; k++)
        {
            //Update mesh
            Scoretext[k].ForceMeshUpdate();
            Mesh mesh = Scoretext[k].mesh;
            Vector3[] vertices = mesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 offset = Wobble(Time.time + i);

                vertices[i] = vertices[i] + offset;
            }

            mesh.vertices = vertices;
            Scoretext[k].canvasRenderer.SetMesh(mesh);

            //Update rotation
            Scoretext[k].transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin((Time.time - k * .25f) * 2.5f) * 2.5f);
        }
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f) * .5f, Mathf.Cos(time * 2.5f) * .5f);
    }
}
