/*
 * Author: Huai
 * Create: 2021/1/14 13:20:23
 *
 * Description:
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Testing
{
    public class CubeManager : MonoBehaviour
    {
        private EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        private void Start()
        {
            Entity cubePrefab = entityManager.CreateEntity(typeof(CubeEntity));
        }
    }
}
