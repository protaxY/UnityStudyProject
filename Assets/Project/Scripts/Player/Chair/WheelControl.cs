using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] float torqueFactor;

    private float _distanseToBottom = 0.0763f;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;

    // Start is called before the first frame update
    void Start()
    {
        _previousPosition = transform.position;
        _previousRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(_previousRotation);

        deltaRotation.ToAngleAxis(out var angle, out var axis);

        angle *= Mathf.Deg2Rad;

        Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;

        float localHorizontalAngularVelocity = transform.InverseTransformDirection(angularVelocity).y;

        RotationControl(localVelocity, localHorizontalAngularVelocity);

        _previousPosition = transform.position;
        _previousRotation = transform.rotation;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanseToBottom + 0.002f);
    }

    private void RotationControl(Vector3 localVelocity, float localHorizontalAngularVelocity)
    {
        if (IsGrounded())
        {           
            Vector3 horizontalLocalVelocity = new Vector3(localVelocity.x, 0f, localVelocity.z);

            float angle = Vector3.Angle(horizontalLocalVelocity, Vector3.forward);
            Vector3 cross = Vector3.Cross(horizontalLocalVelocity, Vector3.forward);
            if (cross.y < 0) angle = -angle;          

            transform.Rotate(new Vector3(0f, -angle * Mathf.Deg2Rad * torqueFactor * localVelocity.magnitude, 0f));

        }
    }

}

