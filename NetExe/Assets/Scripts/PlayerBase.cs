using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class PlayerBase : StrixBehaviour, ICombat
{
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    float shotinterval = 0.1f;
    float interval = 0.1f;
    [SerializeField]
    PanelData.TeamColor team;   //自身のチーム
    public PanelData.TeamColor Team { get { return team; } }

    [SerializeField]
    Texture[] teamTex = new Texture[2];    //チームの色のテクスチャ 
    SkinnedMeshRenderer[] myRenderer;   //自身のメッシュレンダラー

    int posX = 0;   //現在のステージ上のX座標
    int posY = 0;   //現在のステージ上のY座標

    Vector2 input = Vector2.zero;  //スティックの入力
    bool isNeutralX = true; //ニュートラルに戻ったか確認用
    bool isNeutralY = true;

    bool isReady = false;
    public bool isMove = false;    //移動可否フラグ
    public bool isDead = false;    //死亡フラグ


    StageManager stageManager;  //ステージマネージャー
    GameManager gameManager;    //ゲームマネージャー

    // Start is called before the first frame update
    void Start()
    {
        isReady = false;
        myRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        stageManager = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageManager>();
    }

    //ゲーム開始前処理
    void Ready()
    {
        if (isLocal)
        {
            if (StrixNetwork.instance.isRoomOwner)
            {
                team = PanelData.TeamColor.Blue;
            }
            else
            {
                team = PanelData.TeamColor.Red;
            }
            for (int i = 0; i < myRenderer.Length; ++i)
            {
                myRenderer[i].material.mainTexture = teamTex[(int)team];
            }
        }
        else
        {
            if(StrixNetwork.instance.isRoomOwner)
            {
                team = PanelData.TeamColor.Red;
            }
            else 
            {
                team = PanelData.TeamColor.Blue;
            }
            for (int i = 0; i < myRenderer.Length; ++i)
            {
                myRenderer[i].material.mainTexture = teamTex[(int)team];
            }
        }
        //初期位置を決定
        posY = 1;
        if (team == PanelData.TeamColor.Blue)
        {
            posX = 1;
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if(team == PanelData.TeamColor.Red)
        {
            posX = 4;
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        Debug.Log(stageManager);
        Debug.Log(transform.position);
        //ステージデータに自身の位置をセット
        stageManager.PanelDatas[posY, posX].onChara = gameObject;
        transform.position = stageManager.PanelDatas[posY, posX].panelObj.transform.position + new Vector3(0, 0.5f, 0);

        //プレイヤーの準備が完了したらマネージャーに自身をセット
        //gameManager.RpcToAll("SetPlayer", this);
        gameManager.SetPlayer(this);
        isReady = true; //準備完了！
    }

    //上下左右へ移動時の処理
    //_posX,_posY 移動先の座標
    bool Move(int _posX, int _posY)
    {

        if (_posX < 0 || stageManager.PanelDatas.GetLength(1) <= _posX ||
            _posY < 0 || stageManager.PanelDatas.GetLength(0) <= _posY)
        {
            return false; //ステージの範囲外に移動しようしていたら即移動不可とする
        }
        else if (stageManager.PanelDatas[_posY, _posX].onChara != null ||
                stageManager.PanelDatas[_posY, _posX].team != team)
        {
            return false;   //移動先に他のキャラがいる又は、自身のチームのパネルじゃなければ移動不可
        }
        //これらに当てはまらない場合は移動成功,以下移動処理
        stageManager.PanelDatas[posY, posX].onChara = null;
        posX = _posX;
        posY = _posY;
        stageManager.PanelDatas[posY, posX].onChara = gameObject;
        transform.position = stageManager.PanelDatas[posY, posX].panelObj.transform.position + new Vector3(0, 0.5f, 0);

        return true;
    }

    public void SetMove(bool value)
    {
        Debug.Log("SetMove!" + value);
        isMove = value;
    }

    public void Dead()
    {
        Debug.Log("Dead!");
        isDead = true;
    }

    void Shot(int atk)
    {
        interval = 0f;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0,0.5f,0), transform.rotation);
        BulletAction b_act = bullet.GetComponent<BulletAction>();
        Destroy(bullet, 12.0f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward, transform.forward, out hit))
        {
            b_act.FlyLength = Vector3.Distance(hit.transform.position, transform.position);
            b_act.Dir = hit.transform.position - transform.position;
            CombatManager idm = hit.collider.gameObject.GetComponent<CombatManager>();
            if (idm != null && !idm.isLocal)
            {
                Debug.Log("Damageが呼ばれるはず");
                idm.RpcToAll("Damage", atk); //RpcToRoomOwner
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isLocal || isMove)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReady && StrixNetwork.instance.roomSession.IsConnected)
        {
            Ready();    //準備完了でなくて接続完了しているなら
        }
        if (!isLocal || !isMove)
        {
            return;
        }
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (Mathf.Abs(input.x) > 0.3f)
        {
            if (isNeutralX)
            {
                if (input.x > 0)
                {
                    //右に移動
                    Move(posX + 1, posY);
                }
                else
                {
                    //左に移動
                    Move(posX - 1, posY);
                }
                isNeutralX = false;
            }
        }
        else
        {
            isNeutralX = true;
        }
        if (Mathf.Abs(input.y) > 0.3f)
        {
            if (isNeutralY)
            {
                if (input.y > 0)
                {
                    //上に移動
                    Move(posX, posY - 1);
                }
                else
                {
                    //下に移動
                    Move(posX, posY + 1);
                }
                isNeutralY = false;
            }
        }
        else
        {
            isNeutralY = true;
        }
        interval += Time.deltaTime;
        if(Input.GetButtonDown("Fire1") && interval > shotinterval)
        {
            Shot(5);
        }
    }
}