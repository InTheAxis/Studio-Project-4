using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHp = 3;
    public bool dead { private set; get; }
    
    private int hp;
    private bool invulnerable;
    private IEnumerator invulCorr;
    private void Start()
    {
        hp = maxHp;
        dead = false;

        invulCorr = null;
    }

    public void TakeDmg(int dmg)
    {
        if (dead || invulnerable)
            return;

        hp -= dmg;

        Debug.Log("Took dmg, hp is " + hp);

        if (hp <= 0)
        { 
            dead = true;
            hp = 0;
            Debug.Log("Dieded");
            //do dead tings?
        }
    }

    public void SetInvulnerableTime(float dura) 
    {
        if (invulCorr != null)
            StopCoroutine(invulCorr);
        invulCorr = InvulTimeCor(dura);
        StartCoroutine(invulCorr);
    }

    public void SetInvulnerable(bool b)
    {
        invulnerable = b;
    }


    private IEnumerator InvulTimeCor(float dura)
    {
        invulnerable = true;
        yield return new WaitForSeconds(dura);
        invulnerable = false;
    }
}
