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
            var collisionJob = new DamageCollisionJob() {
                DealerDataLookup = SystemAPI.GetComponentLookup<DamageDealerData>(true),
                DealerDestroyFlagLookup = SystemAPI.GetComponentLookup<DamageDealerDestroyFlag>(false),
                DealerBufferLookup = SystemAPI.GetBufferLookup<DamageDealerBuffer>(false),
                ReceiverBufferLookup = SystemAPI.GetBufferLookup<DamageReceiverBuffer>(false),
            };

            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = collisionJob.Schedule(simulationSingleton, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct DamageCollisionJob : ITriggerEventsJob {
        [ReadOnly] public ComponentLookup<DamageDealerData> DealerDataLookup;
        public ComponentLookup<DamageDealerDestroyFlag> DealerDestroyFlagLookup;
        public BufferLookup<DamageDealerBuffer> DealerBufferLookup;
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

            
            var dealerData = DealerDataLookup[dealerEntity];
            var receiverBuffer = ReceiverBufferLookup[receiverEntity];
            var dealerBuffer = DealerBufferLookup[dealerEntity];

            var dealerBufferArray = DealerBufferLookup[dealerEntity].AsNativeArray();
            for (int i = 0; i < dealerBufferArray.Length; i++) {
                if (dealerBufferArray[i].Value == receiverIndex) {
                    return;
                }
            }

            dealerBuffer.Add(new DamageDealerBuffer() { Value = receiverIndex });

            if (dealerBuffer.Length <= dealerData.MaxTargets) {
                receiverBuffer.Add(new DamageReceiverBuffer() { Value = dealerData.Damage });
            }

            
            

            /*
            if (dealerBuffer.Length >= dealerData.MaxTargets) {
                DealerDestroyFlagLookup.SetComponentEnabled(dealerEntity, true);
            }
            */
        }
    }
}
