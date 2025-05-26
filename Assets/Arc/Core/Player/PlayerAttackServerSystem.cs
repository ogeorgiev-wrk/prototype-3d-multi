using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Arc.Core.Damage;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerAttackServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (playerTransform, attackState, attackData, lookInput, attackInput) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerAttackState>, RefRO<PlayerAttackData>, RefRO<PlayerLookInput>, RefRO<PlayerAttackInput>>().WithAll<PlayerTag, Simulate>()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                var isAttacking = attackInput.ValueRO.Value.IsSet;


                if (!isAttacking) {
                    playerTransform.ValueRW.Scale = 1f;
                    attackState.ValueRW.Cooldown = 0f;
                    continue;
                }

                attackState.ValueRW.Cooldown -= deltaTime;
                if (attackState.ValueRW.Cooldown > 0) continue;
                attackState.ValueRW.Cooldown = attackData.ValueRO.AttackRate;                
                

                var attackEntity = ecb.Instantiate(entitiesReferences.AttackPrefabEntity);
                var attackOriginPosition = playerTransform.ValueRO.TransformPoint(attackData.ValueRO.Origin);

                var attackDirection = lookInput.ValueRO.Value - playerTransform.ValueRO.Position;
                var attackTransform = LocalTransform.FromPositionRotation(attackOriginPosition, quaternion.LookRotation(attackDirection, math.up()));
                

                ecb.SetComponent(attackEntity, attackTransform);
                var damageDealerData = new DamageDealerData() {
                    StartPosition = attackTransform.Position,
                    Direction = attackDirection,
                    MaxDistanceSq = math.square(.05f),
                    MoveSpeed = .1f,
                    MaxTargets = 3,
                    Damage = 35,
                };
                ecb.SetComponent(attackEntity, damageDealerData);

                playerTransform.ValueRW.Scale = isAttacking ? 1.2f : 1f;
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
