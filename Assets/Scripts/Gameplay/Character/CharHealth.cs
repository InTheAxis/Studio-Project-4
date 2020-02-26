using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CharHealth : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private CharTPController charControl;
    [SerializeField]
    private CharHitBox hitbox;
    [SerializeField]
    private int maxHp = 3;
    [SerializeField]
    private float autoRespawnTime = 3; //set to negative if dont want auto
    [SerializeField]
    private float respawnInvulTime = 1; //default invul time when respawn

    public delegate void OnHealthChangeCallback(CharTPController playerController, int amtNow);
    public delegate void OnDeadCallback();
    public delegate void OnRespawnCallback();

    public OnHealthChangeCallback OnHealthChange;
    public OnDeadCallback OnDead;
    public OnRespawnCallback OnRespawn;
    public bool dead { private set; get; }
    
    private int hp;
    private bool invulnerable;
    private IEnumerator invulCorr;
    private IEnumerator respawnCorr;

    private void OnEnable()
    {
        hitbox.OnHit += TakeDmg;
    }

    private void OnDisable()
    {
        hitbox.OnHit -= TakeDmg;        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(invulnerable);
            stream.SendNext(dead);
        }
        else 
        {
            int newHp = (int)stream.ReceiveNext();
            if (newHp != hp)
                OnHealthChange?.Invoke(charControl, newHp);
            hp = newHp;

            invulnerable =  (bool)stream.ReceiveNext();
            bool nextDead = (bool)stream.ReceiveNext();
            if (dead && !nextDead)
            {
                dead = nextDead;
                Respawn();
            }
            else if (!dead && nextDead)
            {
                dead = nextDead;
                Die();
            }
        }
    }


    private void Start()
    {
        invulCorr = respawnCorr = null;
        Respawn();
    }

    private void Update()
    {
        //TEMP ,DO THIS ELSEWHERE
        if (Input.GetKeyDown(KeyCode.R))
            Respawn(1);

        if (Input.GetKeyDown(KeyCode.P))
            TakeDmg(1);
    }

    public void Respawn(float invulTime)
    {
        if (respawnCorr != null)
            StopCoroutine(respawnCorr);
        respawnCorr = null;
        //hp = maxHp;
        dead = false;
        Heal(999);
        OnRespawn?.Invoke();
        charControl.disableMovement = dead;
        SetInvulnerableTime(invulTime);
    }
    public void Respawn()
    {
        Respawn(respawnInvulTime);
    }

    public void TakeDmg(int dmg)
    {
        if (dead || invulnerable)
            return;

        hp -= dmg;

        if (hp <= 0)
        {
            hp = 0;
            dead = true;
            Die();
        }

        OnHealthChange?.Invoke(charControl, hp);
        Debug.Log("Took dmg, hp is " + hp);
    }

    public void Heal(int amt)
    {
        if (dead)
            return;

        hp += amt;
        if (hp > maxHp)
            hp = maxHp;

        OnHealthChange?.Invoke(charControl, hp);
        Debug.Log("Healed by " + amt + " , current hp is " + hp);
    }

    private void Die()
    {
        charControl.disableMovement = dead;

        if (autoRespawnTime > 0)
        {
            if (respawnCorr != null)
                StopCoroutine(respawnCorr);
            respawnCorr = AutoRespawn(autoRespawnTime);
            StartCoroutine(respawnCorr);
        }

        OnDead?.Invoke();

        Debug.Log("Dieded");
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
        if (invulCorr != null)
            StopCoroutine(invulCorr);
        invulCorr = null;
    }


    private IEnumerator InvulTimeCor(float dura)
    {
        invulnerable = true;
        yield return new WaitForSeconds(dura);
        invulnerable = false;
    }

    private IEnumerator AutoRespawn(float dura)
    {
        yield return new WaitForSeconds(dura);
        Respawn();
    }
}
