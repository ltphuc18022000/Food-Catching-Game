using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Heart")
        {
           
            Destroy(other.gameObject); 
        }
       
    }
}
