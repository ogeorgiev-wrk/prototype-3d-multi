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
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var deltaTime = SystemAPI.Time.DeltaTime;

            var lifecycleJob = new DamageDealerLifecycleJob() {
                ecbParallel = ecb.AsParallelWriter()
            };
            lifecycleJob.ScheduleParallel();
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            var stateUpdateJob = new DamageDealerStateUpdateJob() {
                DeltaTime = deltaTime,
            };
            stateUpdateJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageDealerTag))]
    public partial struct DamageDealerLifecycleJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ecbParallel;
        public void Execute([ChunkIndexInQuery] int sortKey, in DamageDealerState dealerState, in DamageDealerData dealerData, in DynamicBuffer<DamageDealerBuffer> dealerBuffer, Entity entity) {
            bool shouldDestroy = false;
            bool shouldFlag = false;

            if (dealerState.CurrentDistanceSq > math.square(dealerData.ModifiedParams.MaxDistance)) {
                shouldFlag = true;
                shouldDestroy = true;
            }
            if (dealerBuffer.Length >= dealerData.ModifiedParams.MaxTargets) {
                shouldFlag = true;
            }
            if (dealerState.CurrentLifetime > dealerData.ModifiedParams.MaxLifetime) {
                shouldFlag = true;
                shouldDestroy = true;
            }

            if (shouldFlag) {
                ecbParallel.SetComponentEnabled<DamageDealerNoCollisionFlag>(sortKey, entity, true);
            }

            if (shouldDestroy) {
                ecbParallel.DestroyEntity(sortKey, entity);
            }
        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageDealerTag))]
    public partial struct DamageDealerStateUpdateJob : IJobEntity {
        public float DeltaTime;
        public void Execute(ref PhysicsVelocity physicsVelocity, ref DamageDealerState dealerState, in DamageDealerData dealerData, in LocalTransform transform) {
            var targetVelocity = math.normalize(dealerData.Direction) * dealerData.ModifiedParams.MoveSpeed;
            physicsVelocity.Linear = targetVelocity;
            physicsVelocity.Angular = float3.zero;

            var currentDistanceSq = math.distancesq(transform.Position, dealerData.StartPosition);
            dealerState.CurrentDistanceSq = currentDistanceSq;

            dealerState.CurrentLifetime += DeltaTime;
        }
    }
}
