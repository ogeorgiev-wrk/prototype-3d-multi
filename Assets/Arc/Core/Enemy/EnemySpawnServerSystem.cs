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
        private float _currentTime;
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            _spawnInterval = 100000f;
            _currentTime = 0;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var deltaTime = SystemAPI.Time.DeltaTime;
            _currentTime += deltaTime;
            if (_currentTime < _spawnInterval) return;

            _currentTime = 0;
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
