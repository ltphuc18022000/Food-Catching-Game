using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;
    [Tooltip("Array of sprite transforms that will be used for displaying the countdown until image shot.")]
    public Transform[] countdown;

    [SerializeField] float spawnPeriod = 2f; //spawn period
    public float drag = 2f;
    public float velocity = 1f; //velocity to bottom which makes game harder
    [SerializeField] float periodAdder = 0.1f; //add more fruits every sec

    public Transform[] gameObjects; //diffrent sprites for foods


    //float xMin; //min pos to spawn
    //float xMax; //max pos to spawn
    void Start()
    {
        //Camera gameCamera = Camera.main; //assign main camera to var
        //xMin = gameCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        //xMax = gameCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        //KinectManager manager = KinectManager.Instance;

        //if (manager && manager.IsInitialized() && manager.IsUserDetected(playerIndex))
        //{
        //    StartCoroutine(SpawnContinuously());
        //    StartCoroutine(PeriodAddEverySec());
        //}
        StartCoroutine(CoCountdownPlayGame());

    }
    private IEnumerator CoCountdownPlayGame()
    {
        if (countdown != null && countdown.Length > 0)
        {
            for (int i = 0; i < countdown.Length; i++)
            {
                if (countdown[i])
                    countdown[i].gameObject.SetActive(true);

                yield return new WaitForSeconds(1.0f);

                if (countdown[i])
                    countdown[i].gameObject.SetActive(false);
            }
        }

        PlayGame();
        yield return null;
    }


    /// <summary>
    /// Saves the screen image as png picture, and then opens the saved file.
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(SpawnContinuously());
        StartCoroutine(PeriodAddEverySec());
    }
    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator SpawnContinuously()
    {
        //KinectManager manager = KinectManager.Instance;

        //if (manager && manager.IsInitialized() && manager.IsUserDetected(playerIndex))
    //{
        while (true) //make spawning endless
        {
            var spawnPosX = Random.Range(-3.2f, 3.2f); //pos to spawn randomized beetwen limits

            int randomIndex = Random.Range(0, gameObjects.Length); //chang sprite to random from array

            Transform food = Instantiate(gameObjects[randomIndex], new Vector3(spawnPosX, 5f, 0f), Quaternion.identity) as Transform; //spawning food object
               
            food.GetComponent<Rigidbody>().velocity = new Vector3(0, velocity, 0); //add velocity
            food.GetComponent<Rigidbody>().drag = drag;
            yield return new WaitForSeconds(spawnPeriod); //wait until new spawn

        }
        //}
    }

    IEnumerator PeriodAddEverySec()
    {
        while (true) //endless loop
        {
            periodAdder += 0.1f;
            if (periodAdder > 0.5f) //if period is greater than 0.5 subtract period
            {
                spawnPeriod = spawnPeriod - spawnPeriod / 50; //alghoritm for decreasing period
            }
            if(spawnPeriod <= 0.5f) //if not add velocity
            {
                velocity -= 0.1f; //adds negative velocity
            }
            yield return new WaitForSeconds(1.5f); //do it every 1 sec
        }
    }

}
