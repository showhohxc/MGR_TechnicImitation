using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    Transform TargetTransform;
    // Update is called once per frame
    void Update()
    {
        transform.position = TargetTransform.position;
        //Debug.Log(string.Format("My Character Arm Pos X: {0} ||  Y : {1} ", transform.localPosition.x, transform.localPosition.y) );
    }
}
