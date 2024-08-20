using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public GameObject Barrel, Explosion;

    private AudioSource source;

    [SerializeField]
    private float range;

    private void Awake()
    {
        Barrel.SetActive(true);
        Explosion.SetActive(false);

        source = GetComponent<AudioSource>();
    }

    public void Explode()
    {
        Barrel.SetActive(false);
        Explosion.SetActive(true);
        
        source.Play();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Basket")
        {
            Explode();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
