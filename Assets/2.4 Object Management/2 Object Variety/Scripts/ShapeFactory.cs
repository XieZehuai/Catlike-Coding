/*
 * Author: Huai
 * Create: 2021/1/23 23:13:37
 *
 * Description:
 */

using System.Collections.Generic;
using UnityEngine;

namespace ObjecManagement.ObjectVariety
{
    [CreateAssetMenu(menuName = "2.4 Object Management/2 Object Variety", fileName = "Shape Factory")]
    public class ShapeFactory : ScriptableObject
    {
        [SerializeField] private Shape[] prefabs = default;
        [SerializeField] private Material[] materials = default;
        [SerializeField] private bool recycle = default;

        private List<Shape>[] shapePools;

        public Shape Get(int shapeId = 0, int materialId = 0)
        {
            Shape shape;

            if (recycle)
            {
                if (shapePools == null)
                {
                    CreateShapePools();
                }

                List<Shape> shapePool = shapePools[shapeId];
                int lastIndex = shapePool.Count - 1;

                if (lastIndex >= 0)
                {
                    shape = shapePool[lastIndex];
                    shape.gameObject.SetActive(true);
                    shapePool.RemoveAt(lastIndex);
                }
                else
                {
                    shape = Instantiate(prefabs[shapeId]);
                    shape.ShapeId = shapeId;
                }
            }
            else
            {
                shape = Instantiate(prefabs[shapeId]);
                shape.ShapeId = shapeId;
            }

            shape.SetMaterial(materials[materialId], materialId);
            return shape;
        }

        public Shape GetRandom()
        {
            return Get(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
        }

        public void Reclaim(Shape shape)
        {
            if (recycle)
            {
                if (shapePools == null)
                {
                    CreateShapePools();
                }

                shapePools[shape.ShapeId].Add(shape);
                shape.gameObject.SetActive(false);
            }
            else
            {
                Destroy(shape.gameObject);
            }
        }

        private void CreateShapePools()
        {
            shapePools = new List<Shape>[prefabs.Length];
            for (int i = 0; i < shapePools.Length; i++)
            {
                shapePools[i] = new List<Shape>();
            }
        }
    }
}
