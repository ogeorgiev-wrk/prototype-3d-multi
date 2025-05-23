using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;

namespace Arc.Core.Damage {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct DamageDealerLifecycleServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (dealerState, entity) in SystemAPI.Query<RefRO<DamageDealerState>>().WithEntityAccess()) {
                if (!dealerState.ValueRO.IsMaxRange) continue;
                ecb.SetComponentEnabled<DamageDealerDestroyFlag>(entity, true);
            }

            foreach (var (_, entity) in SystemAPI.Query<EnabledRefRO<DamageDealerDestroyFlag>>().WithEntityAccess()) {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);

            var movementJob = new DamageDealerLifetimeJob();
            movementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageDealerTag))]
    public partial struct DamageDealerLifetimeJob : IJobEntity {
        public void Execute(ref PhysicsVelocity physicsVelocity, ref DamageDealerState dealerState, in DamageDealerData dealerData, in LocalTransform transform) {
            if (dealerState.Direction.Equals(float3.zero)) return;
            if (dealerData.MoveSpeed == 0f) return;
            if (dealerState.IsMaxRange) return;

            physicsVelocity.Angular = float3.zero;

            var currentDistanceSq = math.distancesq(transform.Position, dealerState.StartPosition);
            if (currentDistanceSq >= dealerData.RangeSq) {
                dealerState.IsMaxRange = true;
                physicsVelocity.Linear = float3.zero;
                return;
            }

            float3 moveDirectionNormalized = math.normalize(dealerState.Direction);
            physicsVelocity.Linear = moveDirectionNormalized * dealerData.MoveSpeed;
        }
    }
}
