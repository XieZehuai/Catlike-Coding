/*
 * Author: Huai
 * Create: 2021/1/23 22:17:04
 *
 * Description:
 */

using System;
using System.IO;
using UnityEngine;

namespace ObjecManagement.PersistingObjects
{
    public class DataWriter
    {
        private BinaryWriter writer;

        public DataWriter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        public void Write(int value)
        {
            writer.Write(value);
        }

        public void Write(float value)
        {
            writer.Write(value);
        }

        public void Write(Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public void Write(Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public void Write(Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public void Write(UnityEngine.Random.State value)
        {
            writer.Write(JsonUtility.ToJson(value));
        }
    }
}
