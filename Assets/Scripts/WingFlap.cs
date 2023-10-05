using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class WingFlap : MonoBehaviour
{
    [SerializeField] float maxAngle = 100;
    [SerializeField] public float bendFactor;

    private Transform leftWingTransform;
    private Transform rightWingTransform;

    // Start is called before the first frame update
    void Start()
    {
        leftWingTransform = transform.Find("Armature/wing_l");
        rightWingTransform = transform.Find("Armature/wing_r");
    }

    // Update is called once per frame
    void Update()
    {
        //float weight = GetComponent<Rig>().weight;

        //Debug.Log(leftWingTransform.localEulerAngles);

        leftWingTransform.localEulerAngles = new Vector3(90.0f, 0.0f, -90.0f + bendFactor * maxAngle);
        rightWingTransform.localEulerAngles = new Vector3(90.0f, 0.0f, 90.0f - bendFactor * maxAngle);
    }
}