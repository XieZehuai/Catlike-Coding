/*
 * Author: Huai
 * Create: 2021/1/23 19:05:48
 *
 * Description:
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using ObjecManagement.ObjectVariety;

namespace ObjecManagement.PersistingObjects
{
    public class GameManager : PersistableObject
    {
        public float CreationSpeed { get; set; }
        public float DestructionSpeed { get; set; }

        [SerializeField] private ShapeFactory shapeFactory = default;
        [SerializeField] private PersistentStorage storage = default;

        [SerializeField] private int levelCount = 2;

        [Header("快捷键")]
        [SerializeField] private KeyCode createKey = KeyCode.C;
        [SerializeField] private KeyCode restartKey = KeyCode.R;
        [SerializeField] private KeyCode saveKey = KeyCode.S;
        [SerializeField] private KeyCode loadKey = KeyCode.L;
        [SerializeField] private KeyCode destroyKey = KeyCode.X;

        private const int saveVersion = 2; // the version of the save file

        private List<Shape> shapes;
        private float creationProgress;
        private float destructionProgress;
        private int loadedLevelIndex;

        private void Start()
        {
            shapes = new List<Shape>();

            if (Application.isEditor)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene loadedScene = SceneManager.GetSceneAt(i);
                    if (loadedScene.name.Contains("Level "))
                    {
                        SceneManager.SetActiveScene(loadedScene);
                        loadedLevelIndex = loadedScene.buildIndex;
                        return;
                    }
                }
            }

            StartCoroutine(LoadLevel(1));
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
            else if (Input.GetKeyDown(destroyKey))
            {
                DestroyShape();
            }
            else
            {
                for (int i = 0; i <= levelCount; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                    {
                        Restart();
                        StartCoroutine(LoadLevel(i));
                        return;
                    }
                }
            }

            creationProgress += Time.deltaTime * CreationSpeed;
            // 在帧率很低或者掉帧的时候，deltaTime会变得很大，有可能在一帧
            // 的时间内就增加了很多，甚至超过2，所以要用while循环判断
            while (creationProgress >= 1f)
            {
                creationProgress -= 1f;
                CreateShape();
            }

            destructionProgress += Time.deltaTime * DestructionSpeed;
            while (destructionProgress >= 1f)
            {
                destructionProgress -= 1f;
                DestroyShape();
            }
        }

        private void CreateShape()
        {
            Shape shape = shapeFactory.GetRandom();
            Transform t = shape.transform;

            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Random.Range(0.1f, 1f) * Vector3.one;

            shape.SetColor(Random.ColorHSV(
                hueMin: 0f, hueMax: 1f,
                saturationMin: 0.5f, saturationMax: 1f,
                valueMin: 0.25f, valueMax: 1f,
                alphaMin: 1f, alphaMax: 1f));

            shapes.Add(shape);
        }

        private void DestroyShape()
        {
            if (shapes.Count > 0)
            {
                int index = Random.Range(0, shapes.Count);
                //Destroy(shapes[index].gameObject);
                shapeFactory.Reclaim(shapes[index]);
                int lastIndex = shapes.Count - 1;
                shapes[index] = shapes[lastIndex];
                shapes.RemoveAt(lastIndex);
            }
        }

        private void Restart()
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                shapeFactory.Reclaim(shapes[i]);
            }

            shapes.Clear();
        }

        public override void Save(DataWriter writer)
        {
            writer.Write(shapes.Count);
            writer.Write(loadedLevelIndex);

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
            StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
            for (int i = 0; i < count; i++)
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                Shape shape = shapeFactory.Get(shapeId, materialId);
                shape.Load(reader);
                shapes.Add(shape);
            }
        }

        private IEnumerator LoadLevel(int index)
        {
            enabled = false;
            if (loadedLevelIndex > 0)
            {
                yield return SceneManager.UnloadSceneAsync(loadedLevelIndex);
            }

            yield return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
            loadedLevelIndex = index;
            enabled = true;
        }
    }
}
