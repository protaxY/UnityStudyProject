using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyLimb : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    private ConfigurableJoint m_ConfigurableJoint;

    Quaternion targetInitialRotation;
    // Start is called before the first frame update
    void Start()
    {
        m_ConfigurableJoint = GetComponent<ConfigurableJoint>();
        targetInitialRotation = targetLimb.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        m_ConfigurableJoint.targetRotation = copyRotation();
    }

    private Quaternion copyRotation()
    {
        return Quaternion.Inverse(targetLimb.localRotation) * targetInitialRotation;
    }
}
