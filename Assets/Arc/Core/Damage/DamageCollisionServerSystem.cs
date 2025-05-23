using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
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
                DealerTagLookup = SystemAPI.GetComponentLookup<DamageDealerTag>(true),
                ReceiverTagLookup = SystemAPI.GetComponentLookup<DamageReceiverTag>(true),
                DealerDataLookup = SystemAPI.GetComponentLookup<DamageDealerData>(true),
                DealerDestroyFlagLookup = SystemAPI.GetComponentLookup<DamageDealerDestroyFlag>(false),
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

        [ReadOnly] public ComponentLookup<DamageDealerTag> DealerTagLookup;
        [ReadOnly] public ComponentLookup<DamageReceiverTag> ReceiverTagLookup;
        [ReadOnly] public ComponentLookup<DamageDealerData> DealerDataLookup;
        public ComponentLookup<DamageDealerDestroyFlag> DealerDestroyFlagLookup;
        public BufferLookup<DamageReceiverBuffer> ReceiverBufferLookup;

        public void Execute(TriggerEvent triggerEvent) {
            Entity dealerEntity;
            Entity receiverEntity;

            if (DealerTagLookup.HasComponent(triggerEvent.EntityA) && ReceiverTagLookup.HasComponent(triggerEvent.EntityB)) {
                dealerEntity = triggerEvent.EntityA;
                receiverEntity = triggerEvent.EntityB;
            } else if (DealerTagLookup.HasComponent(triggerEvent.EntityB) && ReceiverTagLookup.HasComponent(triggerEvent.EntityA)) {
                dealerEntity = triggerEvent.EntityB;
                receiverEntity = triggerEvent.EntityA;
            } else {
                return;
            }

            var damageBuffer = ReceiverBufferLookup[receiverEntity];

            var attackDamage = DealerDataLookup[dealerEntity].Damage;
            damageBuffer.Add(new DamageReceiverBuffer() { Value = attackDamage });

            DealerDestroyFlagLookup.SetComponentEnabled(dealerEntity, true);
        }
    }
}
