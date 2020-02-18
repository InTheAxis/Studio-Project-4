using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateBehaviourMessage : UIStateBehaviourBase
{
    [SerializeField]
    private Text messageTextObj;
    [SerializeField]
    private Button okButton;

    public static string messageString { get; set; }

    public void Awake()
    {
        messageTextObj.text = messageString;
        okButton.onClick.AddListener(backToPrevState);
    }
}
