/*
 * Author: Huai
 * Create: 2021/1/23 22:21:40
 *
 * Description:
 */

using System;
using System.IO;
using UnityEngine;

namespace ObjecManagement.PersistingObjects
{
    public class DataReader
    {
        public int Version { get; }

        private BinaryReader reader;

        public DataReader(BinaryReader reader, int version)
        {
            this.reader = reader;
            this.Version = version;
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public Vector3 ReadVector3()
        {
            Vector3 value;
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();

            return value;
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion value;
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            value.w = reader.ReadSingle();

            return value;
        }

        public Color ReadColor()
        {
            Color value;
            value.r = reader.ReadSingle();
            value.g = reader.ReadSingle();
            value.b = reader.ReadSingle();
            value.a = reader.ReadSingle();

            return value;
        }

        public UnityEngine.Random.State ReadRandomState()
        {
            return JsonUtility.FromJson<UnityEngine.Random.State>(reader.ReadString());
        }
    }
}
