using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

public class Check2DMono : MonoBehaviour
{
    public Check2DCollider c2d_collider;
    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        var pos = transform.position;
        if(collider != null)
        {
            if(collider is BoxCollider2D)
            {
            }
            else if(collider is CircleCollider2D)
            {

            }
        }
    }

    void Update()
    {

    }
}
