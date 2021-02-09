/*
 * Author: Huai
 * Create: 2021/1/14 2:13:48
 *
 * Description:
 */

using System;
using UnityEngine;
using Unity.Entities;

namespace Testing
{
    [GenerateAuthoringComponent]
	public struct MovementData : IComponentData
    {
        public int horizontal;
        public int vertical;
        public float speed;
    }
}
