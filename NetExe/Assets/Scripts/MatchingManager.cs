using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要
using SoftGear.Strix.Client.Match.Room.Model;

public class MatchingManager : StrixBehaviour
{ 
    [SerializeField] Image startBtn = null;
    [SerializeField] Image exitBtn = null;
    Outline selOutline = null;
    bool sel;
    float elapsed = 0f;
    [SerializeField]
    public Text p1Name = null;
    [SerializeField]
    public Text p2Name = null;

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
                StrixNetwork.instance.DisconnectMasterServer(); //サーバー切断
                SceneManager.LoadScene("Title");
            }
        }
    }

    [StrixRpc]
    void SetPlayerData()
    {
        Debug.Log("セット呼ばれた");
        if (StrixNetwork.instance.isRoomOwner)
        {
            p1Name.text = "(Owner)" + StrixNetwork.instance.playerName;

            StrixNetwork strixNetwork = StrixNetwork.instance;
            IList<CustomizableMatchRoomMember> roomMembers = StrixNetwork.instance.sortedRoomMembers;
            foreach (var member in roomMembers)
            {
                if (member.GetPrimaryKey() != strixNetwork.selfRoomMember.GetPrimaryKey())
                {
                    p2Name.text = member.GetName();
                }
            }
        }
        else
        {
            StrixNetwork strixNetwork = StrixNetwork.instance;
            IList<CustomizableMatchRoomMember> roomMembers = strixNetwork.sortedRoomMembers;
            foreach (var member in roomMembers)
            {
                if (member.GetPrimaryKey() != strixNetwork.selfRoomMember.GetPrimaryKey())
                {
                    p1Name.text = "(Owner)" + member.GetName();
                }
            }
            p2Name.text = StrixNetwork.instance.playerName;
        }
    }



    [StrixRpc]
    void DeletePlayerData()
    {
        if (isLocal)
        {
            if (StrixNetwork.instance.isRoomOwner)
            {
                p1Name.text = p2Name.text;
                p2Name.text = "";
            }
            else
            {
                p2Name.text = "";
            }
        }
        else
        {
            if (!StrixNetwork.instance.isRoomOwner)
            {
                p2Name.text = "";
            }
            else
            {
                p1Name.text = p2Name.text;
                p2Name.text = "";
            }
        }
    }

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
        SimpleFadeManager.Instance.FadeSceneChange("Main");
    }

}
