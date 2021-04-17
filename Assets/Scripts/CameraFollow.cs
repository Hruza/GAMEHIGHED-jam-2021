using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float rotationSpeed=1;
    [SerializeField] private float followSpeed=1;
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,target.transform.position,followSpeed); 
        transform.position=transform.position-(Vector3.up*transform.position.y);
      //  transform.rotation = Quaternion.Lerp(transform.rotation,target.transform.rotation,rotationSpeed); 
    }
}
