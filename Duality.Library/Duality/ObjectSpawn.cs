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
        [SerializeField] float offset = 0f;

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

            Vector3 position = transform.position;
            position.y += offset;

            var obj = Instantiate(prefab, position, transform.rotation);
            
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
            Vector3 position = transform.position;
            position.y += offset;
            
            target.transform.position = position;
            target.transform.rotation = transform.rotation;

            foreach(var worldObject in target.GetComponents<IWorldObject>())
            {
                worldObject.SetWorld(world);
                worldObject.WrapPosition(position, transform.rotation);
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