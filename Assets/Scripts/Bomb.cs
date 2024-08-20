using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Transform[] countdown;

    [SerializeField] float spawnPeriod = 2f; //spawn period
    [SerializeField] float velocity = 1f; //velocity to bottom which makes game harder
    [SerializeField] float periodAdder = 0.1f; //add more fruits every sec
    public float drag = 2f;
    public Transform[] gameObjects;


    void Start()
    {
        StartCoroutine(CoCountdownPlayGame());

    }

    // Update is called once per frame
    void Update()
    {
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

    public void PlayGame()
    {
        StartCoroutine(SpawnContinuously());
        StartCoroutine(PeriodAddEverySec());
    }

    IEnumerator SpawnContinuously()
    {
       
        while (true) //make spawning endless
        {

            {
                float delay = Random.Range(5f, 10f);
                yield return new WaitForSeconds(delay);
                var spawnPosX = Random.Range(-3f, 3f); //pos to spawn randomized beetwen limits

                int randomIndex = Random.Range(0, gameObjects.Length); //chang sprite to random from array

                Transform bomb = Instantiate(gameObjects[randomIndex], new Vector3(spawnPosX, 6f, 0f), Quaternion.identity) as Transform; //spawning food object

                bomb.GetComponent<Rigidbody>().velocity = new Vector3(0, velocity, 0); //add velocity
                bomb.GetComponent<Rigidbody>().drag = drag;
                yield return new WaitForSeconds(spawnPeriod); //wait until new spawn
            }

        }

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
            if (spawnPeriod <= 0.5f) //if not add velocity
            {
                velocity -= 0.1f; //adds negative velocity
            }
            yield return new WaitForSeconds(3f); //do it every 1 sec
        }
    }
}
