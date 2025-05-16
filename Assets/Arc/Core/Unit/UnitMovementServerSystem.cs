using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Arc.Core.Unit {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UnitMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var unitInitJob = new UnitMovementInitJob();
            unitInitJob.ScheduleParallel();

            var unitMovementJob = new UnitMovementJob();
            unitMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct UnitMovementInitJob : IJobEntity {
        public void Execute(RefRW<PhysicsMass> physicsMass, EnabledRefRW<UnitMovementInitFlag> initFlag) {
            physicsMass.ValueRW.InverseInertia = float3.zero;
            initFlag.ValueRW = false;
        }
    }

    [BurstCompile]
    public partial struct UnitMovementJob : IJobEntity {
        public void Execute(ref PhysicsVelocity physicsVelocity, in UnitMovementSpeed speed, in UnitMovementDirection direction) {
            if (direction.Value.Equals(float3.zero)) return;

            float3 moveDirectionBase = direction.Value;
            float3 moveDirectionNormalized = math.normalize(moveDirectionBase);

            physicsVelocity.Linear = moveDirectionNormalized * speed.Value;
            physicsVelocity.Angular = float3.zero;
        }
    }

}
