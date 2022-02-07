using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAction : MonoBehaviour
{
    float flyLength;    //飛翔距離
    float sumLength;    //飛翔距離の合計
    float speed = 10;
    Vector3 dir;
    public BulletAction(float _flyLength, Vector3 _dir)
    {
        flyLength = _flyLength;
        sumLength = 0;
        dir = _dir;
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
