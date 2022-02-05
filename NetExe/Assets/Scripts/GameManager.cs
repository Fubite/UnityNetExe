using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoftGear.Strix.Unity.Runtime;

public class GameManager : StrixBehaviour
{
    enum GameState
    {
        Start,
        Game,
        Finish,
    }
    [StrixSyncField]
    GameState state;

    //ゲーム上のプレイヤー
    [StrixSyncField]
    PlayerBase[] player;

    //UI関連
    [SerializeField]
    Text naviTxt;


    void Awake()
    {
        player = new PlayerBase[2]; //プレイヤーの実体作成
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Start;
    }

    //プレイヤーをセットする
    [StrixRpc]
    public void SetPlayer(PlayerBase _playey)
    {
        Debug.Log("Set!");
        Debug.Log(_playey.Team.ToString());
        Debug.Log(_playey.gameObject.name);
        player[(int)_playey.Team] = _playey;
    }

    //プレイヤー全員にゲーム開始合図
    [StrixRpc]
    public void GameStart()
    {
        Debug.Log("Start!");
        for (int i = 0; i < player.Length; ++i)
        {
            player[i].SetMove(true);
        }
    }

    //プレイヤー全員にゲーム終了合図
    [StrixRpc]
    public void GameEnd()
    {
        Debug.Log("End");
        for (int i = 0; i < player.Length; ++i)
        {
            player[i].RpcToAll("SetMove", false);
        }
    }
    
    //タイトルへ
    [StrixRpc]
    public void ChangeScene()
    {
        Debug.Log("Change!");
        SceneManager.LoadScene("Matching");
    }


    //開始処理
    IEnumerator Ready()
    {
        naviTxt.text = "Redey?";
        yield return new WaitForSeconds(1.5f);
        naviTxt.text = "Go!";
        RpcToAll("GameStart");
        //GameStart();
        yield return new WaitForSeconds(1);
        naviTxt.text = "";
        yield break;
    }

    //終了処理
    IEnumerator Finish()
    {
        naviTxt.text = "Finish!";
        RpcToAll("GameEnd");
        yield return new WaitForSeconds(1.5f);
        naviTxt.text = (player[1].isDead ? "Red" : "Blue") + " Win!";
        yield return new WaitForSeconds(2f);
        RpcToAll("ChangeScene");
        yield break;
    }


    void Update()
    {
        switch (state)
        {
            case GameState.Start:
                if(player[0] && player[1])  //両プレイヤーが格納されていたら
                {
                    StartCoroutine(Ready());    //開始処理を動かす
                    state = GameState.Game;
                }
                Debug.Log(player[0]?.gameObject.name);
                Debug.Log(player[1]?.gameObject.name);
                break;
            case GameState.Game:
                if (player[0].isDead || player[1].isDead)
                {
                    //赤勝利
                    StartCoroutine(Finish());
                    state = GameState.Finish;
                }
                break;
            case GameState.Finish:

                break;

        }

    }
}
