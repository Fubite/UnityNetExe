using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//パネルデータ
public class PanelData
{
    //各チーム
    public enum TeamColor
    {
        Red,
        Blue,
    }
    public GameObject onChara = null;  //乗っているキャラオブジェクト
    public GameObject panelObj = null; //パネルのオブジェクト
    public TeamColor team;      //パネルのチームの色
}
