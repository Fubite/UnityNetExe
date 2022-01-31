using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //ゲーム上のプレイヤー
    PlayerBase[] player;

    // Start is called before the first frame update
    void Start()
    {
        //ゲーム上のプレイヤー取得処理
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
        player = new PlayerBase[2];
        foreach(var obj in playerObjs)
        {
            PlayerBase playerBase = obj.GetComponent<PlayerBase>();
            if (playerBase)
            {
                if (player[(int)playerBase.Team] == null)
                {
                    player[(int)playerBase.Team] = playerBase;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
