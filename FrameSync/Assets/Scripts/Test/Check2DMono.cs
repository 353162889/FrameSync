using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

public class Check2DMono : MonoBehaviour
{
    public Check2DMono other;
    public Check2DCollider c2d_collider;
    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        var pos = transform.position;
        var up = transform.up;
        var scale = transform.lossyScale;
        if(collider != null)
        {
            if(collider is BoxCollider2D)
            {
                var c = (BoxCollider2D)collider;
                float halfWidth = scale.x * c.size.x / 2;
                float halfHeight = scale.y * c.size.y / 2;
                c2d_collider = new C2D_Rect(new TSVector2(FP.FromFloat(pos.x),FP.FromFloat(pos.y)),new TSVector2(FP.FromFloat(up.x),FP.FromFloat(up.y)),FP.FromFloat(halfWidth),FP.FromFloat(halfHeight));
            }
            else if(collider is CircleCollider2D)
            {
                var c = (CircleCollider2D)collider;
                float radius = Mathf.Max(scale.x, scale.y) * c.radius;
                c2d_collider = new C2D_Circle(new TSVector2(FP.FromFloat(pos.x), FP.FromFloat(pos.y)), new TSVector2(FP.FromFloat(up.x), FP.FromFloat(up.y)), FP.FromFloat(radius));
            }
        }
    }

    void Update()
    {
        if(c2d_collider != null)
        {
            var pos = transform.position;
            c2d_collider.center = new TSVector2(FP.FromFloat(pos.x), FP.FromFloat(pos.y));
        }
        if(other != null)
        {
            if(c2d_collider.CheckCollider(other.c2d_collider))
            {
                CLog.Log("碰撞");
            }
        }
    }
}
