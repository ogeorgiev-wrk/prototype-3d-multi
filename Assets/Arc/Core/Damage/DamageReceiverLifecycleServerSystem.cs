using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;

namespace Arc.Core.Damage {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct DamageReceiverLifecycleServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var ecbParallel = ecb.AsParallelWriter();

            var lifecycleJob = new DamageReceiverLifecycleJob() {
                EcbParallel = ecbParallel
            };
            lifecycleJob.ScheduleParallel();
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageReceiverTag))]
    public partial struct DamageReceiverLifecycleJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter EcbParallel;
        public void Execute([ChunkIndexInQuery] int sortKey, ref DamageReceiverState receiverState, ref DynamicBuffer<DamageReceiverBuffer> receiverBuffer, Entity entity) {
            if (receiverBuffer.IsEmpty) return;
            foreach (var damage in receiverBuffer) {
                receiverState.HealthCurrent -= damage.Value;
            }
            receiverBuffer.Clear();

            if (receiverState.HealthCurrent <= 0) {
                EcbParallel.SetComponentEnabled<DamageReceiverNoCollisionFlag>(sortKey, entity, true);
                EcbParallel.DestroyEntity(sortKey, entity);
            }
        }
    }
}
