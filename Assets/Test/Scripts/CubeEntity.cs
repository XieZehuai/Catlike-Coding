/*
 * Author: Huai
 * Create: 2021/1/14 13:12:17
 *
 * Description:
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Testing
{
    [GenerateAuthoringComponent]
    public class CubeEntity : IComponentData
    {
        public Entity entity;
    }
}
