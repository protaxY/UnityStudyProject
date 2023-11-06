using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcludeColliders : MonoBehaviour
{
    [SerializeField] List<Collider> collidersToIgnore;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider collider in collidersToIgnore)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collider);
        }
    }
}
