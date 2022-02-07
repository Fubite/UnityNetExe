using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoftGear.Strix.Unity.Runtime;

public class CombatManager : StrixBehaviour
{
    Animator myAnim;    //自身のアニメーター
    [SerializeField]
    int maxHP = 100;
    [StrixSyncField, SerializeField]
    public int currentHP = 100;
    //public int CurrentHP => currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        myAnim = GetComponent<Animator>();
    }

    [StrixRpc]  //RPCは自身で実行できない
    public void Damage(int value)
    {
        currentHP -= value;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            myAnim.SetTrigger("Dead");
            GetComponent<ICombat>().Dead();
        }
    }
}
