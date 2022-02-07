using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAction : MonoBehaviour
{
    float flyLength = float.MaxValue;    //飛翔距離
    public float FlyLength
    {
        set { flyLength = value; }
    }

    float sumLength;    //飛翔距離の合計
    float speed = 10f;
    Vector3 dir;
    public Vector3 Dir
    {
        set { dir = value; }
    }
    void Start()
    {
        sumLength = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        sumLength += (dir * speed * Time.deltaTime).magnitude;
        if (sumLength > flyLength)
            Destroy(gameObject);
    }
}
