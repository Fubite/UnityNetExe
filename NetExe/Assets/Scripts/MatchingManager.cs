using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class MatchingManager : MonoBehaviour
{
    [SerializeField] Text p1Name;
    [SerializeField] Text p2Name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinRoom()
    {
        if(StrixNetwork.instance.isRoomOwner)
        {
            p1Name.text = StrixNetwork.instance.playerName;
        }
        else
        {
            p2Name.text = StrixNetwork.instance.playerName;
        }
    }
}
