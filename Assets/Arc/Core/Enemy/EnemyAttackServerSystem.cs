using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Arc.Core.Damage;
using Arc.Core.Player;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct EnemyAttackServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (
                enemyTransform, 
                attackState, 
                attackData
                ) in SystemAPI.Query<RefRW<LocalTransform>, 
                RefRW<EnemyAttackState>, 
                RefRO<EnemyAttackData>
                >().WithAll<EnemyTag, Simulate>()) {

                attackState.ValueRW.CooldownCurrent -= deltaTime;

                bool isOnCooldown = attackState.ValueRO.CooldownCurrent > 0;
                if (isOnCooldown) continue;

                bool isOutOfRange = attackState.ValueRO.DistanceFromTargetSq == 0 || attackState.ValueRO.DistanceFromTargetSq > attackData.ValueRO.DistanceFromTargetMax;
                if (isOutOfRange) continue;

                bool isAttackReady = attackState.ValueRO.CooldownCurrent < 0;
                if (!isAttackReady) continue;

                attackState.ValueRW.CooldownCurrent = attackData.ValueRO.CooldownMax;

                var attackEntity = state.EntityManager.Instantiate(attackData.ValueRO.EntityPrefab);
                var attackSetup = state.EntityManager.GetComponentData<DamageDealerSetup>(attackEntity);
                var attackBaseParams = attackSetup.BaseParams;

                var attackOriginPosition = attackSetup.Source == DamageDealerSource.TARGET ? attackState.ValueRO.TargetPosition : enemyTransform.ValueRO.TransformPoint(attackData.ValueRO.Origin);

                var attackDirection = attackState.ValueRO.TargetPosition - enemyTransform.ValueRO.Position;
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
