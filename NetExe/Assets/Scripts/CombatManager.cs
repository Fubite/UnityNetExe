using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftGear.Strix.Unity.Runtime;
public class CombatManager : StrixBehaviour
{
    Animator myAnim;    //自身のアニメーター
    [SerializeField]
    int maxHP = 100;
    [StrixSyncField]
    int currentHP = 100;

    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        myAnim = GetComponent<Animator>();
        isDead = false;
    }

    [StrixRpc]  //RPCは自身で実行できない
    public void Damage(int value)
    {
        currentHP -= value;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        //hpTxt.text = currentHP.ToString();
        if (currentHP <= 0)
        {
            isDead = true;
        }
    }
    [StrixRpc]
    public void SetHP(int newHP)
    {
        //アニメーターに新ヘルス値を伝達
        myAnim.SetInteger("Health", newHP);
        //新ヘルス値が減少傾向なら、どちらかのモーションを行う
        if (newHP < currentHP)
        {
            if (newHP <= 0)
                myAnim.SetTrigger("Dead");
        }
        if (currentHP != newHP && newHP <= 0)
        {
            Debug.Log("Detected Death !"); //自身の死亡を検出
        }
        currentHP = newHP; //新ヘルス値を現在ヘルスに反映
    }
}
