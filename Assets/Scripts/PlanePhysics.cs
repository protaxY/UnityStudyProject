using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePhysics : MonoBehaviour
{
    [SerializeField] float DragCoefficient;
    [SerializeField] float LiftCoefficient;
    [SerializeField] float verticalSabilizingMomentumCoefficient;
    [SerializeField] float horizontalSabilizingMomentumCoefficient;

    [SerializeField] float softnessOfWings;
    private WingFlap wingFlap;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddRelativeForce(4 * Vector3.forward);
        wingFlap = GetComponent<WingFlap>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        
        Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);
        //Debug.Log(velocity);
        //Debug.Log(rigidbody.velocity);
        Vector3 velocityEulerAngles = Quaternion.FromToRotation(Vector3.forward, velocity).eulerAngles;
        float slidingAngle = velocityEulerAngles.y;
        if (slidingAngle > 180.0)
        {
            slidingAngle = -(360 - slidingAngle);
        }
        //Debug.Log(slidingAngle);
        float angleOfAtack = velocityEulerAngles.x;
        if (angleOfAtack > 180.0)
        {
            angleOfAtack = -(360 - angleOfAtack);
        }

        Vector3 liftForce = new Vector3(0.0f, LiftCoefficient * Mathf.Pow(velocity.z, 2), 0.0f);
        float verticalSabilizingMomentum = verticalSabilizingMomentumCoefficient * angleOfAtack * Mathf.Pow(velocity.magnitude, 2); 
        float horizontalSabilizingMomentum = horizontalSabilizingMomentumCoefficient * slidingAngle * Mathf.Pow(velocity.magnitude, 2);
        
        //Debug.Log(verticalSabilizingMomentum);

        // Mz = mz q S ba, mz - коэффициент момента тангажа, q - скоростной напор, S - площадь крыла, ba - средняя аэродинамическая хорда (САХ)

        rigidbody.AddRelativeForce(liftForce);
        
        rigidbody.AddRelativeTorque(new Vector3(verticalSabilizingMomentum, horizontalSabilizingMomentum, 0.0f));

        wingFlap.bendFactor = Mathf.Clamp(softnessOfWings * velocity.z, 0.0f, 1.0f);
    }
}
