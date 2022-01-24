using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftGear.Strix.Unity.Runtime; //Strixの利用に必要

public class StageManager : StrixBehaviour
{
    [SerializeField]
    Material blueMat;
    [SerializeField]
    Material redMat;

    //パネルの多次元配列
    PanelData[,] panelDatas;
    public PanelData[,] PanelDatas
    {
        get { return panelDatas; }
    }

    private void Start()
    {
        panelDatas = new PanelData[3, 6];
        for (int i = 0; i < panelDatas.GetLength(0); ++i)
        {
            for(int j = 0; j < panelDatas.GetLength(1); ++j)
            {
                panelDatas[i, j] = new PanelData();
                panelDatas[i, j].panelObj = transform.Find("Panel" + (i * panelDatas.GetLength(1) + j)).gameObject;
                if (j <= 2)
                {
                    PanelDataSet(panelDatas[i, j],PanelData.TeamColor.Blue);
                }
                else
                {
                    PanelDataSet(panelDatas[i, j], PanelData.TeamColor.Red);
                }
            }
        }
    }
    //パネルのチームデータセット
    public void PanelDataSet(PanelData data, PanelData.TeamColor _team)
    {
        data.team = _team;
        if (_team == PanelData.TeamColor.Red)
            data.panelObj.GetComponent<Renderer>().material = redMat;
        else
            data.panelObj.GetComponent<Renderer>().material = blueMat;
    }

}
