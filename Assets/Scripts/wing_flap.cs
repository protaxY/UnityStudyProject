using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class wing_flap : MonoBehaviour
{
    private Transform l_bone;
    private Transform r_bone;

    // Start is called before the first frame update
    void Start()
    {

        l_bone = transform.Find("Armature/wing_l");
        r_bone = transform.Find("Armature/wing_r");
    }

    // Update is called once per frame
    void Update()
    {
        float weight = GetComponent<Rig>().weight;
        //l_bone.eulerAngles = new Vector3(90.0f, 0.0f, -90);
        l_bone.eulerAngles = new Vector3(0.0f, 0.0f, -90.0f + weight * 100);
        r_bone.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f - weight * 100);
        //r_bone.eulerAngles = new Vector3(90.0f, 0.0f, 90);


    }
}
