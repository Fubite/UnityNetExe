using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class JoinAction : StrixBehaviour
{
    [SerializeField]
    MatchingManager manager;
    Text myName;
    // Start is called before the first frame update
    void Start()
    {
        myName = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinRoom()
    {
        if (!manager)
            return;
        myName.text = StrixNetwork.instance.playerName;
        if (StrixNetwork.instance.isRoomOwner)
        {
            transform.localPosition = manager.p1Frame.transform.localPosition;
        }
        else
        {
            transform.localPosition = manager.p2Frame.transform.localPosition;
        }
    }

    public void ExitRoom()
    {

    } 
}
