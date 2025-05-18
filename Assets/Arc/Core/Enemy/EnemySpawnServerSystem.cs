using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemySpawnServerSystem : ISystem {
        private float _spawnInterval;
        private float _spawnTimer;
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            _spawnInterval = .5f;
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer > 0) return;
            _spawnTimer = _spawnInterval;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            var enemyEntity = ecb.Instantiate(entitiesReferences.EnemyPrefabEntity);
            var spawnPosition = new float3() { x = UnityEngine.Random.Range(-40, 40), y = 0, z = UnityEngine.Random.Range(-40, 40) };
            ecb.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
