/*
 * Author: Huai
 * Create: 2021/1/29 0:14:01
 *
 * Description:
 */

using UnityEngine;

namespace ObjecManagement.ObjectVariety
{
    public class CompositeSpawnZone : SpawnZone
    {
        [SerializeField] private SpawnZone[] spawnZones = default;

        public override Vector3 SpawnPoint
        {
            get
            {
                int index = Random.Range(0, spawnZones.Length);
                return spawnZones[index].SpawnPoint;
            }
        }
    }
}
