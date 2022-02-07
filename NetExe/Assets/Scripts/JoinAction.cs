using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class JoinAction : StrixBehaviour
{
    [SerializeField]
    MatchingManager manager;
    bool isReady = false;

    public void Update()
    {
        if(!isReady && manager.isSync)
        {
            Debug.Log("セット");
            manager.RpcToAll("SetPlayerData");
            isReady = true;
        }
    }
}
