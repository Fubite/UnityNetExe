using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class MatchingManager : StrixBehaviour
{ 
    [SerializeField] Image startBtn = null;
    [SerializeField] Image exitBtn = null;
    Outline selOutline = null;
    bool sel;
    float elapsed = 0f;
    [SerializeField]
    public Image p1Frame = null;
    [SerializeField]
    public Image p2Frame = null;

    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0f;
        sel = false;
        if (startBtn)
            startBtn.color = new Color(1, 1, 1, 0.5f);
        if (exitBtn)
            selOutline = exitBtn.gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        //RpcToAll("SetName");
        elapsed += Time.deltaTime;
        selOutline.effectColor = new Color(0,0,0,elapsed > 1 ? 2 - elapsed : elapsed);
        if(elapsed > 2f)
        {
            elapsed = 0f;
        }

        //選択処理(オーナーのみ可能)
        if (StrixNetwork.instance.isRoomOwner)
        {
            if (StrixNetwork.instance.room.GetMemberCount() >= 2)
            {
                startBtn.color = new Color(1, 1, 1, 1f);
            }
            else
            {
                startBtn.color = new Color(1, 1, 1, 0.5f);
            }

            float inputy = Input.GetAxis("Vertical");
            if(inputy > 0.3f && StrixNetwork.instance.room.GetMemberCount() >= 2)
            {
                sel = true;
                RpcToAll("SelButton", sel);
            }
            else if(inputy < -0.3f)
            {
                sel = false;
                RpcToAll("SelButton", sel);
            }
        }
        if(Input.GetButtonDown("Submit"))
        {
            if (sel == true)
            {
                RpcToAll("StartGame");
            }
            else
            {
                //ExitRoom();
                SceneManager.LoadScene("Title");
            }
        }
    }

    //public void JoinRoom()
    //{
    //    if (StrixNetwork.instance.isRoomOwner)
    //    {
    //        p1Name = StrixNetwork.instance.selfRoomMember.GetName();
    //    }
    //    else
    //    {
    //        p2Name = StrixNetwork.instance.selfRoomMember.GetName();
    //    }
    //    RpcToAll("SetName");
    //}

    //public void ExitRoom()
    //{
    //    if (StrixNetwork.instance.isRoomOwner)
    //    {
    //        p1Name = p2Name;
    //        p2Name = "Non Player";
    //    }
    //    else
    //    {
    //        p2Name = "Non Player";
    //    }
    //    RpcToAll("SetName");
    //}

    [StrixRpc]
    void SelButton(bool _sel)
    {
        if(_sel)
        {
            selOutline.enabled = false;
            selOutline = startBtn.GetComponent<Outline>();
            selOutline.enabled = true;
        }
        else
        {
            selOutline.enabled = false;
            selOutline = exitBtn.GetComponent<Outline>();
            selOutline.enabled = true;
        }
    }

    [StrixRpc]
    void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

}
