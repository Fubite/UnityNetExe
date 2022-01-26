using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class PlayerBase : StrixBehaviour,IDamage
{
    [SerializeField]
    int maxHP = 100;
    int currentHP = 0;
    [SerializeField]
    PanelData.TeamColor team;   //自身のチーム
    int posX = 0;   //現在のステージ上のX座標
    int posY = 0;   //現在のステージ上のY座標

    Vector2 input = Vector2.zero;  //スティックの入力
    bool isNeutralX = true; //ニュートラルに戻ったか確認用
    bool isNeutralY = true;
    public bool isDead = false;    //死亡フラグ
    Text hpTxt;
    StageManager stageManager;  //ステージマネージャー

    // Start is called before the first frame update
    void Start()
    {
        stageManager = GameObject.FindGameObjectWithTag("Stage").GetComponent<StageManager>();
        hpTxt = GetComponentInChildren<Text>();
        Invoke("Ready", 1f);
    }

    //ゲーム開始前処理
    void Ready()
    {
        currentHP = maxHP;
        hpTxt.text = currentHP.ToString();
        //初期位置を決定
        posY = 1;
        if (team == PanelData.TeamColor.Blue)
        {
            posX = 1;
            hpTxt.canvas.transform.localRotation = Quaternion.Euler(0, -90, 0);
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if(team == PanelData.TeamColor.Red)
        {
            posX = 4;
            hpTxt.canvas.transform.localRotation = Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        //ステージデータに自身の位置をセット
        stageManager.PanelDatas[posY, posX].onChara = gameObject;
        transform.position = stageManager.PanelDatas[posY, posX].panelObj.transform.position + new Vector3(0, 1.5f, 0);
    }

    //上下左右へ移動時の処理
    //_posX,_posY 移動先の座標
    bool Move(int _posX, int _posY)
    {

        if (_posX < 0 || stageManager.PanelDatas.GetLength(1) <= _posX ||
            _posY < 0 || stageManager.PanelDatas.GetLength(0) <= _posY)
        {
            Debug.Log("Out");
            return false; //ステージの範囲外に移動しようしていたら即移動不可とする
        }
        else if (stageManager.PanelDatas[_posY, _posX].onChara != null ||
                stageManager.PanelDatas[_posY, _posX].team != team)
        {
            Debug.Log("Not team");
            return false;   //移動先に他のキャラがいる又は、自身のチームのパネルじゃなければ移動不可
        }
        Debug.Log("Move");
        //これらに当てはまらない場合は移動成功,以下移動処理
        stageManager.PanelDatas[posY, posX].onChara = null;
        posX = _posX;
        posY = _posY;
        stageManager.PanelDatas[posY, posX].onChara = gameObject;
        transform.position = stageManager.PanelDatas[posY, posX].panelObj.transform.position + new Vector3(0, 1.5f, 0);

        return true;
    }

    public void Damage(int value)
    {
        currentHP -= value;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        hpTxt.text = currentHP.ToString();
        if(currentHP <= 0)
        {
            isDead = true;
        }
    }

    void Shot(int atk)
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + transform.forward,transform.forward,out hit))
        {
            hit.collider.gameObject.GetComponent<IDamage>()?.Damage(atk);
        }
    }

    private void FixedUpdate()
    {
        if (!isLocal)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocal)
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

        if(Input.GetButtonDown("Fire1"))
        {
            Debug.Log("shot");
            Shot(5);
        }
    }
}