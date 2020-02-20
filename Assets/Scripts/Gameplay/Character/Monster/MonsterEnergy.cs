using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnergy : MonoBehaviour
{
    [SerializeField]
    private CharHealth health;
    [SerializeField]
    private float maxEnergy = 100;
    [SerializeField] [Tooltip("How many seconds it takes to lose 1 energy")]
    private float decayRate = 0.01f;

    private float energy;

    private void Start()
    {
        RechargeFull();
        health.SetInvulnerable(true);
        StartCoroutine(Decay());
    }

    private IEnumerator Decay()
    {
        while (!health.dead && energy > 0)
        {
            yield return new WaitForSeconds(decayRate);
            energy -= 1.0f;
            if (energy <= 0)
            {
                health.SetInvulnerable(false);
                Debug.Log("Lost all energy");
            }
            Debug.Log("Energy at " +  energy);
        }
    }

    public void Recharge(float amt)
    {
        energy += amt;
        if (energy > maxEnergy)
            energy = maxEnergy;
    }
    public void RechargeFull()
    {
        Recharge(maxEnergy);
    }
}
