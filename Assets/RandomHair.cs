using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int count = transform.childCount;
        int rng=Random.Range(0,count+1);
        if(rng<count+1){
            transform.GetChild(rng).gameObject.SetActive(true);
        }
    }
}
