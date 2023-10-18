using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneDurability : MonoBehaviour
{
    [SerializeField] float durability;
    float damage = 0.0f;

    private Rigidbody _rigidbody;

    float lastForvardVelocity = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentForvardVelocity = transform.InverseTransformDirection(_rigidbody.velocity).z;
        float forvardAcceleration = (currentForvardVelocity - lastForvardVelocity) / Time.fixedDeltaTime;
        lastForvardVelocity = currentForvardVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        float forwardImpulse = transform.InverseTransformDirection(collision.impulse).z;

        damage += durability * (-forwardImpulse);
        damage = Mathf.Clamp(damage, 0.0f, 100.0f);
        GetComponentInChildren<SkinnedMeshRenderer>().SetBlendShapeWeight(0, damage);

        //Debug.Log(durability * (-forwardImpulse));
    }
}
