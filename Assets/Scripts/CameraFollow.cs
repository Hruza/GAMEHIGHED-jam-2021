using System.Collections;
using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float rotationSpeed=1;
    [SerializeField] private float followSpeed=1;

    private Vector3 targetPosition;
    void FixedUpdate()
    {
        if(target!=null){
          targetPosition = new Vector3(target.position.x,Mathf.Max(Mathf.Floor(target.position.y),0),target.position.z);
          transform.position = Vector3.Lerp(transform.position,targetPosition,followSpeed); 
          transform.position = transform.position;
      //  transform.rotation = Quaternion.Lerp(transform.rotation,target.transform.rotation,rotationSpeed); 
        }
    }
}
