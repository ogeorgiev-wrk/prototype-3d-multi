using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Arc.Core.Damage;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct EnemyAttackServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (
                enemyTransform, 
                enemyState, 
                enemyData
                ) in SystemAPI.Query<RefRW<LocalTransform>, 
                RefRW<EnemyState>, 
                RefRO<EnemyData>
                >().WithAll<EnemyTag, Simulate>()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                enemyState.ValueRW.AttackCooldownCurrent -= deltaTime;

                bool isInCooldown = enemyState.ValueRO.AttackCooldownCurrent > 0;
                if (isInCooldown) continue;

                bool isOutOfRange = enemyState.ValueRO.DistanceFromTargetSq > enemyData.ValueRO.AttackRangeSq;
                if (isOutOfRange) continue;

                bool isAttackReady = enemyState.ValueRO.AttackCooldownCurrent <= 0;
                if (!isAttackReady) continue;

                enemyState.ValueRW.AttackCooldownCurrent = enemyData.ValueRO.AttackCooldownMax;
                Debug.Log("ATTACK!");




                var attackEntity = state.EntityManager.Instantiate(enemyData.ValueRO.AttackEntity);
                var attackSetup = state.EntityManager.GetComponentData<DamageDealerSetup>(attackEntity);
                var attackBaseParams = attackSetup.BaseParams;

                var attackOriginPosition = attackSetup.Source == DamageDealerSource.TARGET ? enemyState.ValueRO.TargetPosition : enemyTransform.ValueRO.Position;

                var attackDirection = enemyTransform.ValueRO.Position - enemyState.ValueRO.TargetPosition;
                var attackTransform = LocalTransform.FromPositionRotation(attackOriginPosition, quaternion.LookRotation(attackDirection, math.up()));


                ecb.SetComponent(attackEntity, attackTransform);
                var damageDealerData = new DamageDealerData() {
                    StartPosition = attackTransform.Position,
                    Direction = attackDirection,
                    ModifiedParams = attackBaseParams
                };
                ecb.SetComponent(attackEntity, damageDealerData); 
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
