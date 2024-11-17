using System;
using UnityEngine;

public class ElevatorButtonController : UIButtonController
{
    private const int FLOOR_NUMBER = 5;
    public GameObject[] playerSpawns = new GameObject[FLOOR_NUMBER];
    [NonSerialized] public int CurrentFloor = 0;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        EnableButtons();
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().OnDisable();
    }

    public void TpPlayer(int floor)
    {
        GameObject.FindWithTag("Player").transform.position = playerSpawns[floor].transform.position;
        CurrentFloor = floor;
    }

    // private void Update()
    // {
    //     if (CurrentFocus == CurrentFloor)
    //     {
    //         CurrentFocus++;
    //         SetFocus();
    //     }
    // }

    void EnableButtons()
    {
        // why the fuck i wrote this -x
        // leaving it for wall of shame
        // i spent 15 mins debugging for this shit
        // i hate my life i want this project to end
        // base.Awake();

        for (int i = 0; i < FLOOR_NUMBER; i++)
        {
            Buttons[i].gameObject.SetActive(i != CurrentFloor);
            Buttons[i].RemoveFocus();
        }

        if (CurrentFloor == 0) CurrentFocus = 0;
    }
}
