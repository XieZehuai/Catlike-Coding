/*
 * Author: Huai
 * Create: 2021/1/28 23:50:48
 *
 * Description:
 */

using System;
using UnityEngine;
using ObjecManagement.PersistingObjects;

namespace ObjecManagement.ObjectVariety
{
	public class GameLevel : MonoBehaviour
	{
        [SerializeField] private SpawnZone spawnZone = default;

        private void Start()
        {
            GameManager.Instance.SpawnZoneOfLevel = spawnZone;
        }
    }
}
