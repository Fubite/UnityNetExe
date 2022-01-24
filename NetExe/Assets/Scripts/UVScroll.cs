using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UVScroll : MonoBehaviour
{
    Material myMat;
    Vector2 offset = Vector2.zero;
    float elapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        myMat = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime * 0.01f;
        if (elapsed > 10f)
        {
            elapsed = 0f;
        }
        offset.x = elapsed;
        myMat.mainTextureOffset = offset;
    }
}
