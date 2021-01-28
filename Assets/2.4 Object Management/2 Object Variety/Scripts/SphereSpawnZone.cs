/*
 * Author: Huai
 * Create: 2021/1/29 0:01:35
 *
 * Description:
 */

using UnityEngine;

namespace ObjecManagement.ObjectVariety
{
    public class SphereSpawnZone : SpawnZone
    {
        [SerializeField] private bool surfaceOnly = default;

        public override Vector3 SpawnPoint
        {
            get
            {
                return transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
        }
    }
}
