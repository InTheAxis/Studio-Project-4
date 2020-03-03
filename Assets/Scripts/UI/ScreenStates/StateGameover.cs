using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class StateGameover : State
{
    [SerializeField]
    private TextMeshProUGUI tmInteraction = null;
    [SerializeField]
    private TextMeshProUGUI tmItems = null;
    [SerializeField]
    private TextMeshProUGUI tmRevives = null;
    [SerializeField]
    private TextMeshProUGUI tmTotalScore = null;
    [SerializeField]
    private int countSpeed = 1;

    public static bool isGameover;

    public override string Name { get { return "Gameover"; } }

    private void Awake()
    {
        StateController.Register(this);

        if (isGameover)
        {
            isGameover = false;
            PhotonNetwork.LeaveRoom();

            StateController.showNext(Name);
        }
    }
    private void OnDestroy()
    {
        StateController.Unregister(this);
    }

    private void Update()
    {
    
    }

    public void backToLobby()
    {
        StateController.showNext("Mainmenu");
    }

    public void Quit()
    {

    }

    private IEnumerator countScore(TextMeshProUGUI tmp, int targetScore, float delay=0.0f)
    {
        yield return new WaitForSeconds(delay);
        int score = 0;
        tmp.text = "+" + score.ToString();

        //float countWait = countDuration / ((float)targetScore * 2.0f);
        //Debug.Log(countWait);
        int increment = (targetScore / 100) * countSpeed;

        while (score < targetScore)
        {
            score += increment;
            tmp.text = "+" + score.ToString();
            yield return new WaitForEndOfFrame();
        }
        score = targetScore;
        tmp.text = "+" + score.ToString();
    }

    public override void onShow()
    {
        base.onShow();

        StartCoroutine(countScore(tmInteraction, 3000));
        StartCoroutine(countScore(tmItems, 500));
        StartCoroutine(countScore(tmRevives, 1200));
        StartCoroutine(countScore(tmTotalScore, 3000+500+1200, 1.8f));
    }
}
