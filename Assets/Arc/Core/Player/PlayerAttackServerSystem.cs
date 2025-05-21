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

            foreach (var (localTransform, playerInput, playerEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerAttackInput>>().WithAll<Simulate>().WithEntityAccess()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                var isPrimary = playerInput.ValueRO.PrimaryAttack.IsSet;
                var isSecondary = playerInput.ValueRO.SecondaryAttack.IsSet;
                if (!isPrimary && !isSecondary) {
                    localTransform.ValueRW.Scale = 1f;
                    continue;
                }
                

                var attackEntity = ecb.Instantiate(entitiesReferences.AttackPrefabEntity);
                var originOffset = state.EntityManager.GetComponentData<PlayerAttackOrigin>(playerEntity).Value;
                var spawnPosition = localTransform.ValueRO.TransformPoint(originOffset);


                var playerDirection = state.EntityManager.GetComponentData<UnitLookDirection>(playerEntity).Value;

                var attackOrigin = LocalTransform.FromPositionRotation(spawnPosition, localTransform.ValueRO.Rotation);

                ecb.SetComponent(attackEntity, attackOrigin);
                ecb.SetComponent(attackEntity, new AttackState() {
                    StartPosition = attackOrigin.Position,
                    Direction = playerDirection,
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
