/*
 * Author: Huai
 * Create: 2021/1/23 19:05:48
 *
 * Description:
 */

using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using ObjecManagement.ObjectVariety;

namespace ObjecManagement.PersistingObjects
{
    public class GameManager : PersistableObject
    {
        [SerializeField] private ShapeFactory shapeFactory = default;
        [SerializeField] private PersistentStorage storage = default;

        [Header("¿ì½Ý¼ü")]
        [SerializeField] private KeyCode createKey = KeyCode.C;
        [SerializeField] private KeyCode restartKey = KeyCode.R;
        [SerializeField] private KeyCode saveKey = KeyCode.S;
        [SerializeField] private KeyCode loadKey = KeyCode.L;

        private const int saveVersion = 1; // the version of the save file
        private List<Shape> shapes;

        private void Awake()
        {
            shapes = new List<Shape>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateShape();
            }
            else if (Input.GetKeyDown(restartKey))
            {
                Restart();
            }
            else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this, saveVersion);
            }
            else if (Input.GetKeyDown(loadKey))
            {
                storage.Load(this);
            }
        }

        private void CreateShape()
        {
            Shape shape = shapeFactory.GetRandom();
            Transform t = shape.transform;

            t.localPosition = Random.insideUnitCircle * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Random.Range(0.1f, 1f) * Vector3.one;

            shape.SetColor(Random.ColorHSV(
                hueMin: 0f, hueMax: 1f,
                saturationMin: 0.5f, saturationMax: 1f,
                valueMin: 0.25f, valueMax: 1f,
                alphaMin: 1f, alphaMax: 1f));

            shapes.Add(shape);
        }

        private void Restart()
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                Destroy(shapes[i].gameObject);
            }

            shapes.Clear();
        }

        public override void Save(DataWriter writer)
        {
            writer.Write(shapes.Count);

            for (int i = 0; i < shapes.Count; i++)
            {
                writer.Write(shapes[i].ShapeId);
                writer.Write(shapes[i].MaterialId);
                shapes[i].Save(writer);
            }
        }

        public override void Load(DataReader reader)
        {
            Restart();

            int version = reader.Version;
            if (version > saveVersion)
            {
                Debug.LogError("Unsupported future save version " + version);
                return;
            }

            int count = version <= 0 ? -version : reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                Shape shape = shapeFactory.Get(shapeId, materialId);
                shape.Load(reader);
                shapes.Add(shape);
            }
        }
    }
}
