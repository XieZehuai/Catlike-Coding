/*
 * Author: Huai
 * Create: 2021/1/23 22:31:12
 *
 * Description:
 */

using System;
using System.IO;
using UnityEngine;

namespace ObjecManagement.PersistingObjects
{
	public class PersistentStorage : MonoBehaviour
	{
        private const string FILENAME = "Save";
        private string savePath;

        private void Awake()
        {
            savePath = Path.Combine(Application.persistentDataPath, FILENAME);
            Debug.Log("´æµµÂ·¾¶ " + savePath);
        }

        public void Save(PersistableObject obj)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                obj.Save(new DataWriter(writer));
            }
        }

        public void Load(PersistableObject obj)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
            {
                obj.Load(new DataReader(reader));
            }
        }
    }
}
