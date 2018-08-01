using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Proto;
using UnityEngine;

namespace Game
{
    public static class GameInTool
    {
        private static TSRandom mRandom;
        private static uint mStartUnitId;
        private static uint mStartRemoteId;
        private static List<TSVector> mListTSVector = new List<TSVector>();
        private static List<Vector3> mListUnityVector = new List<Vector3>();
        public static void InitRandomSeed(int seed)
        {
            mRandom = TSRandom.New(seed);
            mStartUnitId = 0;
            mStartRemoteId = 0;
        }

        public static uint GenerateUnitId()
        {
            return ++mStartUnitId;
        }

        public static uint GenerateRemoteId()
        {
            return ++mStartRemoteId;
        }

        public static FP Random(FP max)
        {
            return mRandom.Next(max);
        }

        public static FP Random(FP min,FP max)
        {
            return mRandom.Next(min, max);
        }

        public static int Random(int max)
        {
            return mRandom.Next(max);
        }

        public static int Random(int min,int max)
        {
            return mRandom.Next(min, max);
        }

        public static TSVector RandomInRect(TSRect rect)
        {
            FP x = Random(rect.xMin, rect.xMax);
            FP z = Random(rect.yMin, rect.yMax);
            return new TSVector(x,0,z);
        }

        public static TSVector ToTSVector(ProtoVector2 source)
        {
            TSVector result = new TSVector();
            result.x = FP.FromSourceLong(source.x);
            result.y = 0;
            result.z = FP.FromSourceLong(source.y);
            return result;
        }

        public static ProtoVector2 ToProtoVector2(TSVector source)
        {
            ProtoVector2 result = new ProtoVector2();
            result.x = source.x.ToSourceLong();
            result.y = source.z.ToSourceLong();
            return result;
        }

        public static List<TSVector> ToLstTSVector(List<ProtoVector2> source)
        {
            mListTSVector.Clear();
            for (int i = 0; i < source.Count; i++)
            {
                mListTSVector.Add(ToTSVector(source[i]));
            }
            return mListTSVector;
        }

        public static List<Vector3> TSVectorToLstUnityVector3(List<TSVector> source)
        {
            mListUnityVector.Clear();
            for (int i = 0; i < source.Count; i++)
            {
                mListUnityVector.Add(source[i].ToUnityVector3());
            }
            return mListUnityVector;
        }
    }
}
