using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Arc.Core.Enemy;
using Unity.Collections;

namespace Arc.Core.Attack {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    partial struct AttackCollisionServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var attackCollisionJob = new AttackCollisionJob() {
                AttackTagLookup = SystemAPI.GetComponentLookup<AttackTag>(true),
                TargetTagLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
                AttackStateLookup = SystemAPI.GetComponentLookup<AttackState>(false)
            };

            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = attackCollisionJob.Schedule(simulationSingleton, state.Dependency);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct AttackCollisionJob : ITriggerEventsJob {

        [ReadOnly] public ComponentLookup<AttackTag> AttackTagLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> TargetTagLookup;
        public ComponentLookup<AttackState> AttackStateLookup;

        public void Execute(TriggerEvent triggerEvent) {
            Entity attackEntity;
            Entity targetEntity;

            if (AttackTagLookup.HasComponent(triggerEvent.EntityA) && TargetTagLookup.HasComponent(triggerEvent.EntityB)) {
                attackEntity = triggerEvent.EntityA;
                targetEntity = triggerEvent.EntityB;
            } else if (AttackTagLookup.HasComponent(triggerEvent.EntityB) && TargetTagLookup.HasComponent(triggerEvent.EntityA)) {
                attackEntity = triggerEvent.EntityB;
                targetEntity = triggerEvent.EntityA;
            } else {
                return;
            }

            var component = AttackStateLookup[attackEntity];
            component.ShouldDestroy = true;

            AttackStateLookup[attackEntity] = component;
        }
    }
}
