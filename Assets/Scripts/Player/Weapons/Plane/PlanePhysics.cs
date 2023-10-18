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

    private Rigidbody _rigidbody;
    private WingFlap wingFlap;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        wingFlap = GetComponent<WingFlap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 velocity = transform.InverseTransformDirection(_rigidbody.velocity);
        Vector3 velocityEulerAngles = Quaternion.FromToRotation(Vector3.forward, velocity).eulerAngles;

        float slidingAngle = velocityEulerAngles.y;
        float angleOfAtack = velocityEulerAngles.x;
        if (slidingAngle > 180.0)
        {
            slidingAngle = -(360 - slidingAngle);
        }
        if (angleOfAtack > 180.0)
        {
            angleOfAtack = -(360 - angleOfAtack);
        }

        Vector3 liftForce = new Vector3(0.0f, LiftCoefficient * Mathf.Pow(velocity.z, 2), 0.0f);
        float verticalSabilizingMomentum = verticalSabilizingMomentumCoefficient * angleOfAtack * Mathf.Pow(velocity.magnitude, 2);
        float horizontalSabilizingMomentum = horizontalSabilizingMomentumCoefficient * slidingAngle * Mathf.Pow(velocity.magnitude, 2);

        // Mz = mz q S ba, mz - коэффициент момента тангажа, q - скоростной напор, S - площадь крыла, ba - средняя аэродинамическая хорда (САХ)

        _rigidbody.AddRelativeForce(liftForce);

        _rigidbody.AddRelativeTorque(new Vector3(verticalSabilizingMomentum, horizontalSabilizingMomentum, 0.0f));

        wingFlap.bendFactor = Mathf.Clamp(softnessOfWings * velocity.z, 0.0f, 1.0f);
    }
}
