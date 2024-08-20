using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RorateSelection : MonoBehaviour
{
    private float zPos;
    // Start is called before the first frame update
    void Start()
    {
        zPos = transform.eulerAngles.z;
        StartCoroutine(rotate(100f));

    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator rotate(float value)
    {
        zPos+=value;
        while(transform.eulerAngles.z!=zPos)
        {
            //  transform.rotation=Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f,0f,zPos),0.1f*Time.deltaTime);
            var r = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(r.x, r.y, Mathf.LerpAngle(r.z, zPos, 0.1f * Time.deltaTime));
            yield return null;

        }
    }
}
