/*
 * Author: Huai
 * Create: 2021/1/14 2:09:31
 *
 * Description:
 */

using System;
using UnityEngine;
using Unity.Entities;

namespace Testing
{
    [GenerateAuthoringComponent]
	public struct InputData : IComponentData
    {
        public KeyCode upKey;
        public KeyCode downKey;
        public KeyCode leftKey;
        public KeyCode rightKey;
    }
}
