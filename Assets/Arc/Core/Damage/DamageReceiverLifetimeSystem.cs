using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;

namespace Arc.Core.Damage {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct DamageReceiverLifetimeServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI.Query<EnabledRefRO<DamageReceiverDestroyFlag>>().WithEntityAccess()) {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);

            foreach (var (receiverState, receiverBuffer, entity) in SystemAPI.Query<RefRW<DamageReceiverState>, DynamicBuffer<DamageReceiverBuffer>>().WithPresent<DamageReceiverDestroyFlag>().WithEntityAccess()) {
                if (receiverBuffer.IsEmpty) continue;
                foreach (var damage in receiverBuffer) {
                    receiverState.ValueRW.HealthCurrent -= damage.Value;
                }
                receiverBuffer.Clear();

                if (receiverState.ValueRW.HealthCurrent <= 0) {
                    SystemAPI.SetComponentEnabled<DamageReceiverDestroyFlag>(entity, true);
                }
            }

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
