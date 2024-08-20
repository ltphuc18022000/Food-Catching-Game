using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class Lose : MonoBehaviour
{
    [SerializeField] GameObject engine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Food")
        {
            engine.GetComponent<Engine>().lifes--; //subtract life if collsion with ground
            Destroy(other.gameObject); //destroy fruit
        }
        if (other.GetComponent<Collider>().tag == "Bomb")
        {
            Destroy(other.gameObject);
        }
        if (other.GetComponent<Collider>().tag == "Heart")
        {
            Destroy(other.gameObject);
        }
        if (other.GetComponent<Collider>().tag == "Clock")
        {
            Destroy(other.gameObject);
        }
    }

}
