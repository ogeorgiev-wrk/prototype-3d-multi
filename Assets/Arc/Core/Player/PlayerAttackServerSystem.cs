using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Arc.Core.Attack;

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

            foreach (var (localTransform, attackState, attackData, lookDirection, playerInput) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerAttackState>, RefRO<PlayerAttackData>, RefRO<UnitLookDirection>, RefRO<PlayerAttackInput>>().WithAll<Simulate>()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                var isPrimary = playerInput.ValueRO.PrimaryAttack.IsSet;
                var isSecondary = playerInput.ValueRO.SecondaryAttack.IsSet;


                attackState.ValueRW.Cooldown -= deltaTime;
                if (attackState.ValueRW.Cooldown > 0 && isSecondary) continue;
                attackState.ValueRW.Cooldown = attackData.ValueRO.AttackRate;

                
                if (!isPrimary && !isSecondary) {
                    localTransform.ValueRW.Scale = 1f;
                    continue;
                }
                

                var attackEntity = ecb.Instantiate(entitiesReferences.AttackPrefabEntity);
                var spawnPosition = localTransform.ValueRO.TransformPoint(attackData.ValueRO.Origin);
                var attackOrigin = LocalTransform.FromPositionRotation(spawnPosition, localTransform.ValueRO.Rotation);

                ecb.SetComponent(attackEntity, attackOrigin);
                ecb.SetComponent(attackEntity, new AttackState() {
                    StartPosition = attackOrigin.Position,
                    Direction = lookDirection.ValueRO.Value,
                });

                localTransform.ValueRW.Scale = isPrimary ? 1.2f : .8f;
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
