using UnityEngine;

public class ElevatorButton : UIButton
{
    private static readonly int OnClose = Animator.StringToHash("OnClose");

    public override void Perform()
    {
        transform.parent.GetComponent<ElevatorButtonController>().CurrentFloor = ID;
        transform.parent.GetComponent<Animator>().SetTrigger(OnClose);
        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().OnEnable();
        
        base.Perform();
    }
}
