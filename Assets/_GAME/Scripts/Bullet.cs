using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Lean.Pool;
public class Bullet : MonoBehaviour
{
    //Bullets gain the ability to instantiate after a certain amount of time.
    [Range(0.1f, 200f)]
    [SerializeField] private float bulletSpeed = 6f;
    [Range(0.1f, 1f)]
    [SerializeField] private float despawnShield = 0.3f;
    [SerializeField] private float originDestroyTimer;
    [Tooltip("Defines how many frames a bullet needs to replenish its multiplying ability.")]
    [Range(0, 30)]
    [SerializeField] private float instantiateWaitAmount = 3;
    [SerializeField] private int bounceLevel = 0, pierceLevel = 0;
    [SerializeField] private int bounceLeft = 0, pierceLeft = 0;

    private bool isInstantiatable = false;
    private float remainingTimeToDestroy = 0f;
    private Rigidbody rigidbody;
    private bool despawnChecker = false;


    public float ResetRemainingTime
    {
        set { remainingTimeToDestroy = value; }
    }
    public int PierceLevel
    {
        set { pierceLeft = pierceLevel = value; }
    }
    public int BounceLevel
    {
        set { bounceLeft = bounceLevel = value; }
    }

    public int DecreasePierce
    {
        set
        {
            pierceLeft--;
            if (pierceLeft < 0)
                Despawner();

        }
    }

    public int DecreaseBounce
    {
        set
        {
            bounceLeft--;
            if (bounceLeft < 0)
                Despawner();

        }
    }

    public bool IsInstantiatable
    {
        get { return isInstantiatable; }
        set { isInstantiatable = value; }
    }

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(SpawnWaiter());

    }

    void OnBecameInvisible() => despawnChecker = true;

    private void OnBecameVisible() => despawnChecker = false;


    void Update()
    {
        if (despawnShield <= 0f) Despawner();
        if (despawnChecker) despawnShield -= Time.deltaTime;
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime, Space.Self);
        rigidbody.angularDrag = 0f;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Time.deltaTime * bulletSpeed + 0.5f))
        {
            if (hit.transform.gameObject.tag == "Wall")
                if (bounceLeft > 0)
                {
                    DecreaseBounce = 1;
                    Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                    float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                    transform.eulerAngles = new Vector3(0, rot, 0);
                }
                else
                {
                    Despawner();
                }
            else if (hit.transform.gameObject.tag == "Enemy")
                DecreasePierce = 1;
        }
    }   

    private void Despawner()
    {
        remainingTimeToDestroy = 0f;
        IsInstantiatable = true;
        despawnShield = 0.3f;
        //LeanPool.Despawn(this.gameObject);
        Destroy(this.gameObject);
    }

    IEnumerator SpawnWaiter()
    {
        IsInstantiatable = false;
        for (int i = 0; i < instantiateWaitAmount; i++)
        {
            yield return null;
        }
        IsInstantiatable = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Portal")
            despawnShield = 0.3f;
    }

}