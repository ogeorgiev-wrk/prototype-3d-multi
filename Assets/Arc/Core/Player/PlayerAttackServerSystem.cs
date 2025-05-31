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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (
                playerTransform, 
                attackState, 
                attackData, 
                targetInput, 
                attackInput, 
                swapInput,
                attackPrefabBuffer
                ) in SystemAPI.Query<RefRW<LocalTransform>, 
                RefRW<PlayerAttackState>, 
                RefRO<PlayerAttackData>, 
                RefRO<PlayerTargetInput>, 
                RefRO<PlayerAttackInput>, 
                RefRO<PlayerWeaponSwapInput>,
                DynamicBuffer<PlayerAttackPrefabBuffer>
                >().WithAll<PlayerTag, Simulate>()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                var isAttacking = attackInput.ValueRO.Value.IsSet;

                if (!isAttacking) {
                    playerTransform.ValueRW.Scale = 1f;
                    continue;
                }

                var attackEntity = state.EntityManager.Instantiate(attackPrefabBuffer[swapInput.ValueRO.Value].Value);
                var attackSetup = state.EntityManager.GetComponentData<DamageDealerSetup>(attackEntity);
                var attackBaseParams = attackSetup.BaseParams;

                var attackModifiers = attackState.ValueRO.Modifiers;

                var attackOriginPosition = attackSetup.Source == DamageDealerSource.TARGET ? targetInput.ValueRO.Value : playerTransform.ValueRO.TransformPoint(attackData.ValueRO.Origin);

                var attackDirection = targetInput.ValueRO.Value - playerTransform.ValueRO.Position;
                var attackTransform = LocalTransform.FromPositionRotation(attackOriginPosition, quaternion.LookRotation(attackDirection, math.up()));
                

                ecb.SetComponent(attackEntity, attackTransform);
                var damageDealerData = new DamageDealerData() {
                    StartPosition = attackTransform.Position,
                    Direction = attackDirection,
                    ModifiedParams = new DamageDealerParams() {
                        Damage = attackBaseParams.Damage * attackModifiers.Damage,
                        MoveSpeed = attackBaseParams.MoveSpeed * attackModifiers.MoveSpeed,
                        MaxTargets = attackBaseParams.MaxTargets * attackModifiers.MaxTargets,
                        MaxDistance = attackBaseParams.MaxDistance * attackModifiers.MaxDistance,
                        MaxLifetime = attackBaseParams.MaxLifetime * attackModifiers.Lifetime,
                        TriggerStart = attackBaseParams.TriggerStart * attackModifiers.Lifetime,
                        TriggerEnd = attackBaseParams.TriggerEnd * attackModifiers.Lifetime,
                    }
                };
                ecb.SetComponent(attackEntity, damageDealerData);


                playerTransform.ValueRW.Scale = isAttacking ? 1.2f : 1f;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
