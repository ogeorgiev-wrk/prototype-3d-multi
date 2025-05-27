using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Collections;

namespace Arc.Core.Damage {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    partial struct DamageCollisionServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            simulationSingleton.AsSimulation().FinalJobHandle.Complete();

            var elapsedTime = SystemAPI.Time.ElapsedTime;

            var collisionTriggerJob = new DamageCollisionTriggerJob() {
                ElapsedTime = elapsedTime,

                DealerDataLookup = SystemAPI.GetComponentLookup<DamageDealerData>(true),
                DealerNoCollisionFlag = SystemAPI.GetComponentLookup<DamageDealerNoCollisionFlag>(true),
                DealerBufferLookup = SystemAPI.GetBufferLookup<DamageDealerBuffer>(false),

                ReceiverCollisionTimeLookup = SystemAPI.GetComponentLookup<DamageReceiverCollisionTime>(false),
                ReceiverNoCollisionFlag = SystemAPI.GetComponentLookup<DamageReceiverNoCollisionFlag>(true),
                ReceiverBufferLookup = SystemAPI.GetBufferLookup<DamageReceiverBuffer>(false),
            };
            var collisionTriggerHandle = collisionTriggerJob.Schedule(simulationSingleton, state.Dependency);
            collisionTriggerHandle.Complete();

            var collisionStateJob = new DamageCollisionStateJob() {
                ElapsedTime = elapsedTime,
            };
            collisionStateJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct DamageCollisionStateJob : IJobEntity {
        [ReadOnly] public double ElapsedTime;

        public void Execute(ref DamageReceiverCollisionState collisionState, in DamageReceiverCollisionTime collisionTime) {
            bool isCollision = collisionTime.Value == ElapsedTime;

            var newState = CollisionState.None;

            if (isCollision && collisionState.Value == CollisionState.None) {
                newState = CollisionState.Enter;
            }

            if (isCollision && collisionState.Value == CollisionState.Enter) {
                newState = CollisionState.Inside;
            }

            if (isCollision && collisionState.Value == CollisionState.Inside) {
                newState = CollisionState.Inside;
            }

            if (!isCollision && collisionState.Value == CollisionState.Inside) {
                newState = CollisionState.Exit;
            }

            collisionState.Value = newState;
        }
    }

    [BurstCompile]
    public partial struct DamageCollisionTriggerJob : ITriggerEventsJob {
        [ReadOnly] public double ElapsedTime;

        [ReadOnly] public ComponentLookup<DamageDealerData> DealerDataLookup;
        [ReadOnly] public ComponentLookup<DamageDealerNoCollisionFlag> DealerNoCollisionFlag;
        public BufferLookup<DamageDealerBuffer> DealerBufferLookup;

        public ComponentLookup<DamageReceiverCollisionTime> ReceiverCollisionTimeLookup;
        [ReadOnly] public ComponentLookup<DamageReceiverNoCollisionFlag> ReceiverNoCollisionFlag;
        public BufferLookup<DamageReceiverBuffer> ReceiverBufferLookup;

        public void Execute(TriggerEvent triggerEvent) {
            
            Entity dealerEntity;
            Entity receiverEntity;

            int dealerIndex;
            int receiverIndex;

            if (DealerDataLookup.HasComponent(triggerEvent.EntityA) && ReceiverBufferLookup.HasBuffer(triggerEvent.EntityB)) {
                dealerEntity = triggerEvent.EntityA;
                dealerIndex = triggerEvent.BodyIndexA;

                receiverEntity = triggerEvent.EntityB;
                receiverIndex = triggerEvent.BodyIndexB;
            } else if (DealerDataLookup.HasComponent(triggerEvent.EntityB) && ReceiverBufferLookup.HasBuffer(triggerEvent.EntityA)) {
                dealerEntity = triggerEvent.EntityB;
                dealerIndex = triggerEvent.BodyIndexB;

                receiverEntity = triggerEvent.EntityA;
                receiverIndex = triggerEvent.BodyIndexA;
            } else {
                return;
            }

            //bool ignoreCollisions = DealerNoCollisionFlag.IsComponentEnabled(dealerEntity) || ReceiverNoCollisionFlag.IsComponentEnabled(receiverEntity);
            //if (ignoreCollisions) return;
            
            var dealerData = DealerDataLookup[dealerEntity];
            var dealerBuffer = DealerBufferLookup[dealerEntity];

            var receiverBuffer = ReceiverBufferLookup[receiverEntity];
            ReceiverCollisionTimeLookup[receiverEntity] = new DamageReceiverCollisionTime() { Value = ElapsedTime };

            var dealerBufferArray = DealerBufferLookup[dealerEntity].AsNativeArray();
            for (int i = 0; i < dealerBufferArray.Length; i++) {
                if (dealerBufferArray[i].Value == receiverIndex) {
                    return;
                }
            }

            dealerBuffer.Add(new DamageDealerBuffer() { Value = receiverIndex });

            if (dealerBuffer.Length <= dealerData.ModifiedParams.MaxTargets) {
                receiverBuffer.Add(new DamageReceiverBuffer() { Value = dealerData.ModifiedParams.Damage });
            }
        }
    }
}
