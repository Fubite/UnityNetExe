using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleFadeManager : SingletonMonoBehaviour<SimpleFadeManager>
{
    [SerializeField, Header("フェードのカラー")] Color fadeColor = Color.black;
    [SerializeField, Header("フェードする時間(初期値)")] float fadeTime = 1.0f;
    [SerializeField, Header("フェードの為のImage")] Image fadeImage;

    public bool IsFade { get { return fadeCoroutine != null; } }
    public Color FadeColor { set { fadeColor = value; } }

    //今動いているコルーチン
    Coroutine fadeCoroutine;
    Coroutine fadeActionCoroutine;
    private new void Awake()
    {
        base.Awake();
        if(this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// フェード本体:
    /// fadeinTime =  フェードにかかる時間,
    /// isIn =      true フェードイン, false フェードアウト
    /// </summary>
    public void Fade(float _fadeTime = -1.0f, bool _isIn = true)
    {
        if(fadeCoroutine != null)
        {
            FadeWarning();
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeCoroutine(_fadeTime, _isIn));
    }

    /// <summary>
    /// フェードした後にアクション実行:
    /// action =    実行したいアクション,
    /// fadeinTime =  フェードにかかる時間,
    /// isIn =      true フェードイン, false フェードアウト
    /// </summary>
    public void FadeAction(System.Action _action, float _fadeTime = -1.0f, bool _isIn = false)
    {
        if (fadeActionCoroutine != null)
        {
            FadeWarning();
            StopCoroutine(fadeActionCoroutine);
        }
        fadeActionCoroutine = StartCoroutine(FadeActionCoroutine(_action, _fadeTime, _isIn));
    }
    /// <summary>
    /// フェードしながらシーン切り替え:
    /// sceneName =   切り替え先のシーン,
    /// fadeinTime =    フェードにかかる時間
    /// </summary>
    public void FadeSceneChange(string _sceneName, float _fadeTime = -1.0f)
    {
        FadeAction(() => StartCoroutine(FadeSceneChangeCoroutine(_sceneName, _fadeTime)), _fadeTime);
    }
    //フェードしながらシーン切り替え(in out)
    IEnumerator FadeSceneChangeCoroutine(string _sceneName, float _fadeTime = -1.0f)
    {
        yield return SceneManager.LoadSceneAsync(_sceneName);
        Fade(_fadeTime);
    }
    //フェードした後にアクション実行
    IEnumerator FadeActionCoroutine(System.Action _action, float _fadeTime, bool _isIn)
    {
        //フェード中なら停止
        if (fadeCoroutine != null)
        {
            FadeWarning();
            StopCoroutine(fadeCoroutine);
        }
        //フェード
        fadeCoroutine = StartCoroutine(FadeCoroutine(_fadeTime, _isIn));
        //フェードしているのを待つ
        yield return fadeCoroutine;
        fadeActionCoroutine = null;
        //アクション実行
        _action?.Invoke();
    }
    //フェード本体
    IEnumerator FadeCoroutine(float _fadeTime, bool _isIn)
    {
        //フェードタイムが0以下なら初期設定を使う
        if (_fadeTime <= 0)
        {
            _fadeTime = fadeTime;
        }
        //終わりのカラーを設定する
        Color endColor = fadeColor;

        float startTime = Time.time, div, rate;
        //isInがtrueならフェードインfalseならアウト
        fadeColor.a = System.Convert.ToInt32(_isIn);
        endColor.a = System.Convert.ToInt32(!_isIn);
        //いったんフェードカラーにする
        fadeImage.color = fadeColor;
        while (true)
        {
            //コルーチン開始からの時間
            div = Time.time - startTime;
            //0～1までの時間経過
            rate = div / _fadeTime;
            //フェード
            fadeImage.color = Color.Lerp(fadeColor, endColor, rate);
            //rateが1異常なら終了
            if (rate >= 1)
            {
                fadeCoroutine = null;
                yield break;
            }
            yield return null;
        }
    }

    void FadeWarning()
    {
#if UNITY_EDITOR
        Debug.LogWarning("フェード中にフェードを呼び出さないでください。");
#endif
    }
}
