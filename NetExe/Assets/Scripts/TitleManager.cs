using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    enum BUTTONS
    { 
        TITLE,
        RULE,
        QUIT
    }

    BUTTONS selectButton;

    float elapsed = 0;

    [SerializeField,Header("ボタン選択のインターバル")]float interval = 0.5f;

    [SerializeField,Header("ボタンたち")]Image[] buttons = new Image[3];

    // Start is called before the first frame update
    void Start()
    {
        SelectButton(BUTTONS.TITLE);

        SoundManager.Instance.BgmPlayer.Play("Title");
        SoundManager.Instance.BgmPlayer.Volume = 0.5f;
        
    }

    void SelectButton(BUTTONS _selectButton)
    {
        buttons[(int)selectButton].GetComponent<Animator>().SetBool("Select", false);
        selectButton = _selectButton;
        buttons[(int)selectButton].GetComponent<Animator>().SetBool("Select", true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            switch(selectButton)
            {
                case BUTTONS.TITLE:
                    SimpleFadeManager.Instance.FadeSceneChange("Matching");
                    SoundManager.Instance.SePlayer.Play("Decesion");
                    break;
                case BUTTONS.RULE:
                    break;
                case BUTTONS.QUIT:
                    Application.Quit();
                    break;
            }
        }

        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(v) >= 0.2f)
        {
            elapsed -= Time.deltaTime;

            if (elapsed <= 0)
            {
                elapsed = interval;
                SoundManager.Instance.SePlayer.Play("Select");

                if (v >= 0.2f)
                {
                    if (selectButton > BUTTONS.TITLE)
                    {
                        SelectButton(selectButton - 1);
                    }
                }
                else
                {
                    if (selectButton < BUTTONS.QUIT)
                    {
                        SelectButton(selectButton + 1);
                    }
                }
            }
        }
        else
        {
            elapsed = 0.0f;
        }
    }
}
