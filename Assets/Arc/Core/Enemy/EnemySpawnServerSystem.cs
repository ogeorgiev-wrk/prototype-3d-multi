using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Arc.Core.Player;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemySpawnServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (spawnerState, spawnData, transform) in SystemAPI.Query<RefRW<EnemySpawnerState>, RefRO<EnemySpawnerData>, RefRO<LocalTransform>>().WithAll<EnemySpawnerTag, Simulate>()) {
                spawnerState.ValueRW.Cooldown -= deltaTime;
                if (spawnerState.ValueRW.Cooldown > 0) continue;

                spawnerState.ValueRW.Cooldown = spawnData.ValueRO.SpawnRate;
                var enemyEntity = ecb.Instantiate(spawnData.ValueRO.EnemyPrefabEntity);

                var spawnAngle = spawnerState.ValueRW.Random.NextFloat(0f, math.TAU);
                var spawnPoint = new float3 {
                    x = math.sin(spawnAngle),
                    y = 0f,
                    z = math.cos(spawnAngle)
                };
                var spawnOffset = spawnPoint * spawnerState.ValueRW.Random.NextFloat(spawnData.ValueRO.SpawnRadiusMin, spawnData.ValueRO.SpawnRadiusMax);

                var spawnPosition = transform.ValueRO.Position + spawnOffset;
                ecb.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
