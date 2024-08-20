using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static Unity.VisualScripting.Member;

public class Basket : MonoBehaviour
{
    [SerializeField] GameObject engine;


    [SerializeField] float padding = 1f; //padding for catcher

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip catchSound;
    [SerializeField] AudioClip trapSound;
    [SerializeField] AudioClip heartSound;
    [SerializeField] AudioClip FreezeSound;
    //public bool freeze = false;
    public Bomb BombSpawner;
    public FoodSpawner FoodSpawner;
    //max and min for x and y on screen
    float xMin;
    float xMax;

    float yMin;
    float yMax;

    //public float slow;
    //private float starttimescale;
    //private float deltatime;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries(); //set limit for catcher
        //startTimeScale = Time.timeScale;
        //deltaTime = Time.fixedDeltaTime;

    }

    // Update is called once per frame
    void Update()
    {
        //if (freeze == true)
        //{
        //    StartSlow();

        //}
        //if (freeze == false)
        //{
        //    StopSlow();
        //}  

    }

    //private void StartSlow()
    //{
    //    Time.timeScale = slow;
    //    Time.fixedDeltaTime = deltaTime * slow;
    //}
    //private void StopSlow()
    //{
    //    Time.timeScale = startTimeScale;
    //    Time.fixedDeltaTime = deltaTime;
    //}

      

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Food")
        {
            int score = Random.Range(1, 3); //randomize score points

            engine.GetComponent<Engine>().score += score; //adds score

            engine.GetComponent<Engine>().LiveAdderForScore += score; //adds score to life adder score
            audioSource.PlayOneShot(catchSound); //play sfx after catch
            Destroy(other.gameObject); //remove the fruit object
        }
        if (other.GetComponent<Collider>().tag == "Bomb")
        {
            int score = Random.Range(-3,-1);
            engine.GetComponent<Engine>().score += score;
            audioSource.PlayOneShot(trapSound);
            other.GetComponent<MeshCollider>().enabled = false;    
        }
        if (other.GetComponent<Collider>().tag == "Heart")
        {
            int life = 1;
            engine.GetComponent<Engine>().lifes += life;
            audioSource.PlayOneShot(heartSound);
            Destroy(other.gameObject);
        }
        if (other.GetComponent<Collider>().tag == "Clock")
        {
            //freeze = true;

            FoodSpawner.drag = 8f;
            BombSpawner.drag = 8f;
            StartCoroutine(ReturnVelocity());

            //timeManager.GetComponent<TimeManager>().SlowMotion();
            audioSource.PlayOneShot(FreezeSound);

            Destroy(other.gameObject);
        }
        
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main; //assign main camera to variable
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0.2f, 0f)).y;
    }
      
    IEnumerator ReturnVelocity()
    {
        yield return new WaitForSeconds(10f);
        FoodSpawner.drag = 2f;
    }    
    
}
