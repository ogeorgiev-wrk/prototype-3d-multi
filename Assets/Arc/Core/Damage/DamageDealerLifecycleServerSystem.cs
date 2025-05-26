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
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (dealerState, dealerData, dealerBuffer, entity) in SystemAPI.Query<RefRO<DamageDealerState>, RefRO<DamageDealerData>, DynamicBuffer<DamageDealerBuffer>>().WithEntityAccess()) {
                bool shouldDestroy = false;
                if (dealerState.ValueRO.DistanceCurrentSq >= dealerData.ValueRO.MaxDistanceSq) shouldDestroy = true;
                //if (dealerBuffer.Length >= dealerData.ValueRO.MaxTargets) shouldDestroy = true;

                if (shouldDestroy) ecb.SetComponentEnabled<DamageDealerDestroyFlag>(entity, true);
            }

            foreach (var (_, entity) in SystemAPI.Query<EnabledRefRO<DamageDealerDestroyFlag>>().WithEntityAccess()) {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);

            var movementJob = new DamageDealerMovementJob() {
                DeltaTime = deltaTime,
            };
            movementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageDealerTag))]
    [WithDisabled(typeof(DamageDealerDestroyFlag))] 
    public partial struct DamageDealerMovementJob : IJobEntity {
        public float DeltaTime;
        public void Execute(ref PhysicsVelocity physicsVelocity, ref DamageDealerState dealerState, in DamageDealerData dealerData, in LocalTransform transform) {
            if (dealerData.Direction.Equals(float3.zero)) return;
            if (dealerData.MoveSpeed == 0f) return;

            var targetVelocity = math.normalize(dealerData.Direction) * dealerData.MoveSpeed;
            physicsVelocity.Linear = targetVelocity;
            physicsVelocity.Angular = float3.zero;

            var currentDistanceSq = math.distancesq(transform.Position, dealerData.StartPosition);
            dealerState.DistanceCurrentSq = currentDistanceSq;
        }
    }
}
