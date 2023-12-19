using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class WingFlap : MonoBehaviour
{
    [SerializeField] float maxAngle = 100;
    [SerializeField] public float bendFactor;

    [SerializeField] private Transform leftWingTransform;
    [SerializeField] private Transform rightWingTransform;

    // Update is called once per frame
    void Update()
    {
        leftWingTransform.localEulerAngles = new Vector3(90.0f, 0.0f, -90.0f + bendFactor * maxAngle);
        rightWingTransform.localEulerAngles = new Vector3(90.0f, 0.0f, 90.0f - bendFactor * maxAngle);
    }
}
