using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Duality
{
    public class ObjectSpawn : MonoBehaviour
    {
        [SerializeField] GameObject prefab = null;
        [SerializeField] bool spawnOnStart = true;
        [SerializeField] World world = World.White;

        private void Start()
        {
            if (spawnOnStart)
            {
                this.Spawn();
            }
        }

        public void Spawn()
        {
            this.Spawn(prefab);
        }

        public void Spawn(GameObject prefab)
        {
            if (prefab is null)
            {
                return;
            }

            var obj = Instantiate(prefab, transform.position, transform.rotation);
            
            // Set all objects requiring a spawn point to this one
            foreach (var spawn in obj.GetComponents<IHasSpawnPoint>())
            {
                spawn.Spawn = this;
            }

            foreach(var worldObject in obj.GetComponents<IWorldObject>())
            {
                worldObject.SetWorld(world);
            }
        }

        public void Respawn(GameObject target)
        {
            target.transform.position = transform.position;
            target.transform.rotation = transform.rotation;

            foreach(var worldObject in target.GetComponents<IWorldObject>())
            {
                worldObject.SetWorld(world);
            }
        }

        public void RespawnAfter(GameObject target, float time)
        {
            if (time <= 0f)
            {
                Respawn(target);
            }
            else 
            {
                StartCoroutine(HandleDelayedRespawn(target, time));
            }
        }

        private IEnumerator HandleDelayedRespawn(GameObject target, float time)
        {
            target.gameObject.SetActive(false);
            yield return new WaitForSeconds(time);
            target.gameObject.SetActive(true);
            Respawn(target);
        }
    }
}