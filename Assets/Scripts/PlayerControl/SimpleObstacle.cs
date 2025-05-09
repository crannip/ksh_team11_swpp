using System;
using UnityEngine;

public class SimpleObstacle : MonoBehaviour
{
    public float knockbackConst;
    public float stunTime;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerBehavior p = other.gameObject.GetComponent<PlayerBehavior>();
            
            
            p.GetHit((other.transform.position - transform.position).normalized * knockbackConst + Vector3.up * knockbackConst, stunTime);
        }
    }
}
