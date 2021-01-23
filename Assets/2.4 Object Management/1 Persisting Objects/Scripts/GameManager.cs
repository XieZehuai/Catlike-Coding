/*
 * Author: Huai
 * Create: 2021/1/23 19:05:48
 *
 * Description:
 */

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjecManagement.PersistingObjects
{
    public class GameManager : PersistableObject
    {
        [SerializeField] private PersistableObject prefab = default;
        [SerializeField] private PersistentStorage storage = default;

        [Header("¿ì½Ý¼ü")]
        [SerializeField] private KeyCode createKey = KeyCode.C;
        [SerializeField] private KeyCode restartKey = KeyCode.R;
        [SerializeField] private KeyCode saveKey = KeyCode.S;
        [SerializeField] private KeyCode loadKey = KeyCode.L;

        private List<PersistableObject> objects;

        private void Awake()
        {
            objects = new List<PersistableObject>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateObject();
            }
            else if (Input.GetKeyDown(restartKey))
            {
                Restart();
            }
            else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this);
            }
            else if (Input.GetKeyDown(loadKey))
            {
                storage.Load(this);
            }
        }

        private void CreateObject()
        {
            PersistableObject obj = Instantiate(prefab);
            Transform t = obj.transform;

            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Random.Range(0.1f, 1f) * Vector3.one;

            objects.Add(obj);
        }

        private void Restart()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                Destroy(objects[i].gameObject);
            }

            objects.Clear();
        }

        public override void Save(DataWriter writer)
        {
            writer.Write(objects.Count);

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Save(writer);
            }
        }

        public override void Load(DataReader reader)
        {
            Restart();
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PersistableObject obj = Instantiate(prefab);
                obj.Load(reader);
                objects.Add(obj);
            }
        }
    }
}
