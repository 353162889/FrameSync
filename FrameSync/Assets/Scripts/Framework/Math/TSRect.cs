using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public struct TSRect
    {
        public FP x;
        public FP y;
        public FP width;
        public FP height;

        public static readonly TSRect zero;

        static TSRect()
        {
            zero = new TSRect(0, 0, 0, 0);
        }

        public TSRect(int x,int y,int width,int height)
        {
            this.x = (FP)x;
            this.y = (FP)y;
            this.width = (FP)width;
            this.height = (FP)height;
        }

        public TSRect(FP x,FP y,FP width,FP height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public FP xMin
        {
            get { return x; }
            set { x = value; }
        }

        public FP yMin
        {
            get { return y; }
            set { y = value; }
        }

        public FP xMax
        {
            get { return x + width; }
            set { width = value - x; }
        }

        public FP yMax
        {
            get { return y + height; }
            set { height = value - y; }
        }

        public FP xCenter
        {
            get { return x + width / 2; }
            set { x = value - width / 2; }
        }
        public FP yCenter
        {
            get { return y + height / 2; }
            set { y = value - height / 2; }
        }
        public FP halfWidth { get { return width / 2; } }
        public FP halfHeight { get { return height / 2; } }

        public static bool operator ==(TSRect rect1,TSRect rect2)
        {
            return rect1.x == rect2.x && rect1.y == rect2.y && rect1.width == rect2.width && rect1.height == rect2.height;
        }

        public static bool operator !=(TSRect rect1,TSRect rect2)
        {
            return !(rect1 == rect2);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ width.GetHashCode() ^ height.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0:f1}, {1:f1}, {2:f1}, {3:f1})", x.AsFloat(), y.AsFloat(), width.AsFloat(),height.AsFloat());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TSRect)) return false;
            TSRect other = (TSRect)obj;

            return (((x == other.x) && (y == other.y)) && (width == other.width) && (height == other.height));
        }

        public bool Contains(TSVector2 position)
        {
            return !(position.x < xMin || position.x > xMax || position.y < yMin || position.y > yMax);
        }

        public bool Contains(FP x,FP y)
        {
            return !(x < xMin || x > xMax || y < yMin || y > yMax);
        }

        public static TSRect FromUnityRect(Rect rect)
        {
            return new TSRect(FP.FromFloat(rect.x),FP.FromFloat(rect.y),FP.FromFloat(rect.width),FP.FromFloat(rect.height));
        }

        public Rect ToUnityRect()
        {
            return new Rect(this.x.AsFloat(),this.y.AsFloat(),this.width.AsFloat(),this.height.AsFloat());
        }
    }
}
