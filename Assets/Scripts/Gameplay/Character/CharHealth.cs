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
    private float autoRespawnTime = -1; //set to negative if dont want auto
    [SerializeField]
    private float respawnInvulTime = 1; //default invul time when respawn
    [SerializeField]
    private float hitstunTime = 1; //default invul time when respawn

    public delegate void OnHealthChangeCallback(CharTPController playerController, int amtNow, bool isInvulnerable);
    public delegate void OnDeadCallback(int viewId);
    public delegate void OnRespawnCallback();

    public OnHealthChangeCallback OnHealthChange;
    public OnDeadCallback OnDead;
    public OnRespawnCallback OnRespawn;
    public bool dead { get; private set; }
    
    public int hp { get; private set; }
    public bool invulnerable { get; private set; }
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
            SetInvulnerable((bool)stream.ReceiveNext());
            bool nextDead = (bool)stream.ReceiveNext();

            if (newHp != hp)
                OnHealthChange?.Invoke(charControl, newHp, invulnerable);
            hp = newHp;

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
        if (charControl.photonView.IsMine && PhotonNetwork.IsConnected)
            if (Input.GetKeyDown(KeyCode.Alpha9))
                Respawn(1);
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
        charControl.DisableMovement(dead);
        charControl.rb.isKinematic = false;
        SetInvulnerableTime(invulTime);
    }
    public void Respawn()
    {
        Respawn(respawnInvulTime);
    }

    public void TakeDmg(int dmg, float dot)
    {
        if (dead)
            return;

        if (charControl.photonView.IsMine && PhotonNetwork.IsConnected)
        { 
            CharTPCamera.Instance.Shake();
            StartCoroutine(HitStun());
        }

        if (invulnerable)
            return;

        hp -= dmg;

        if (hp <= 0)
        {
            hp = 0;
            dead = true;
            Die();
        }

        OnHealthChange?.Invoke(charControl, hp, invulnerable);
        Debug.LogFormat("{0} took dmg, hp is {1}", charControl.gameObject.name, hp);
    }

    public void Heal(int amt)
    {
        if (dead)
            return;

        hp += amt;
        if (hp > maxHp)
            hp = maxHp;

        OnHealthChange?.Invoke(charControl, hp, invulnerable);
        Debug.Log("Healed by " + amt + " , current hp is " + hp);
    }

    private void Die()
    {
        charControl.DisableMovement(dead);
        charControl.rb.isKinematic = true;

        if (autoRespawnTime > 0)
        {
            if (respawnCorr != null)
                StopCoroutine(respawnCorr);
            respawnCorr = AutoRespawn(autoRespawnTime);
            StartCoroutine(respawnCorr);
        }

        OnDead?.Invoke(PhotonView.Get(charControl).ViewID);
        Debug.LogFormat("Sending ID, {0}", PhotonView.Get(charControl).ViewID);

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
        OnHealthChange?.Invoke(charControl, hp, invulnerable);
        if (invulCorr != null)
            StopCoroutine(invulCorr);
        invulCorr = null;
    }


    private IEnumerator InvulTimeCor(float dura)
    {
        invulnerable = true;
        OnHealthChange?.Invoke(charControl, hp, invulnerable);
        yield return new WaitForSeconds(dura);
        invulnerable = false;
        OnHealthChange?.Invoke(charControl, hp, invulnerable);
    }

    private IEnumerator AutoRespawn(float dura)
    {
        yield return new WaitForSeconds(dura);
        Respawn();
    }

    private IEnumerator HitStun()
    {
        charControl.DisableMovement(true);
        charControl.rb.isKinematic = true;
        yield return new WaitForSeconds(hitstunTime);
        if (!dead)
        {
            charControl.DisableMovement(false);
            charControl.rb.isKinematic = false;
        }
    }

    [PunRPC]
    public void takeDmgRPC(int dmg, float dot)
    {
        TakeDmg(dmg, dot);
    }
}
