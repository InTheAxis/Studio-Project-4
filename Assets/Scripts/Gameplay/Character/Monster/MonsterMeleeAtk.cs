using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterMeleeAtk : MonoBehaviour
{
    [SerializeField]
    private CharTPController charControl;
    [SerializeField]
    private DamageData dmgData;
    [SerializeField]
    private float attackDuration = 0.3f;
    [SerializeField]
    private float attackRate = 0.3f;

    private bool pressed;
    private IEnumerator corr;
    private float timer;

    private void Start()
    {
        corr = null;
        timer = 0;
        Debug.LogWarning("Remove charcontrol==null");
    }

    private void Update()
    {
        if (charControl == null || (!charControl.photonView.IsMine && PhotonNetwork.IsConnected))
            return;

        pressed = Input.GetAxisRaw("Fire2") != 0;

        timer += Time.deltaTime;

        if (pressed && timer >= attackRate)
        {
            timer = 0;
            if (corr != null)
                StopCoroutine(corr);
            corr = StartDmg();
            StartCoroutine(corr);
        }
    }

    private IEnumerator StartDmg()
    {
        dmgData.SetIsDamaging(gameObject);
        yield return new WaitForSeconds(attackDuration);
        dmgData.SetNotDamaging();
    }
}
