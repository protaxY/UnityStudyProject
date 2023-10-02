using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CubeControl : MonoBehaviour
{
    private float terminateDistance;
    private float squareTerminateDistance;
    private GameObject deleter;

    CubeControl()
    {
        Debug.Log("Created");

        terminateDistance = 0.1f;
        squareTerminateDistance = Mathf.Pow(terminateDistance, 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        deleter = GameObject.Find("Deleter");
    }

    // Update is called once per frame
    void Update()
    {
        if ((deleter.transform.position - transform.position).sqrMagnitude <= squareTerminateDistance)
        {
            Destroy(gameObject);
        }

        Vector3 movementDirection = new Vector3(0.0f, 0.0f, 0.0f);
        float speed = 10;

        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementDirection += Vector3.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection += Vector3.back;
        }

        transform.position += Time.deltaTime * speed * movementDirection;
    }
}
