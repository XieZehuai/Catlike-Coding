/*
 * Author: Huai
 * Create: 2021/1/23 23:13:37
 *
 * Description:
 */

using UnityEngine;

namespace ObjecManagement.ObjectVariety
{
    [CreateAssetMenu(menuName = "2.4 Object Management/2 Object Variety", fileName = "Shape Factory")]
    public class ShapeFactory : ScriptableObject
    {
        [SerializeField] private Shape[] prefabs = default;
        [SerializeField] private Material[] materials = default;

        public Shape Get(int shapeId = 0, int materialId = 0)
        {
            Shape shape = Instantiate(prefabs[shapeId]);
            shape.ShapeId = shapeId;
            shape.SetMaterial(materials[materialId], materialId);
            return shape;
        }

        public Shape GetRandom()
        {
            return Get(Random.Range(0, prefabs.Length), Random.Range(0, materials.Length));
        }
    }
}
