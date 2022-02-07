using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoftGear.Strix.Unity.Runtime;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        Start,
        Game,
        Finish,
    }
    GameState state;
    
    //ゲーム上のプレイヤー
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
    public void SetPlayer(PlayerBase _playey)
    {
        player[(int)_playey.Team] = _playey;
    }

    //プレイヤー全員にゲーム開始合図
    public void GameStart()
    {
        Debug.Log("Start!");
        for (int i = 0; i < player.Length; ++i)
        {
            player[i].SetMove(true);
        }
    }

    //プレイヤー全員にゲーム終了合図
    public void GameEnd()
    {
        Debug.Log("End");
        for (int i = 0; i < player.Length; ++i)
        {
            player[i].SetMove(false);
        }
    }
    
    //タイトルへ
    public void ChangeScene()
    {
        StrixNetwork.instance.DisconnectMasterServer(); //サーバー切断
        SceneManager.LoadScene("Title");
    }


    //開始処理
    IEnumerator Ready()
    {
        naviTxt.text = "Redey?";
        yield return new WaitForSeconds(1.5f);
        naviTxt.text = "Go!";
        GameStart();
        yield return new WaitForSeconds(1);
        naviTxt.text = "";
        yield break;
    }

    //終了処理
    IEnumerator Finish()
    {
        naviTxt.text = "Finish!";
        GameEnd();
        yield return new WaitForSeconds(1.5f);
        naviTxt.text = (player[1].isDead ? "Red" : "Blue") + " Win!";
        yield return new WaitForSeconds(2f);
        ChangeScene();
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
                    StartCoroutine(Finish());   //終了処理を動かす
                    state = GameState.Finish;
                }
                break;
            case GameState.Finish:

                break;

        }

    }
}
