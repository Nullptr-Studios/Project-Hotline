using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  EnemyDamageable : Damageable
{
    public GameObject bloodEffectManager;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void DoDamage(float amount, Vector3 shootDir, Vector3 hitPoint)
    {
        base.DoDamage(amount, shootDir, hitPoint);

        GameObject BManager = Instantiate(bloodEffectManager, hitPoint, new Quaternion());
        BManager.transform.right = shootDir;
    }
}
