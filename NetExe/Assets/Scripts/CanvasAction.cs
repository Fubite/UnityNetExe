using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasAction : MonoBehaviour
{
    PlayerBase myPlayer;

    [SerializeField]
    Text txtHP;

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = transform.parent.gameObject.GetComponent<PlayerBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!myPlayer) return;

        txtHP.text = myPlayer.CurrentHP.ToString("f0");

        transform.forward = Camera.main.transform.forward;
    }
}
