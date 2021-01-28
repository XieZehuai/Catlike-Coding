/*
 * Author: Huai
 * Create: 2021/1/28 23:15:29
 *
 * Description:
 */

using UnityEngine;

namespace ObjecManagement.ObjectVariety
{
	public abstract class SpawnZone : MonoBehaviour
	{
        public abstract Vector3 SpawnPoint { get; }
    }
}
