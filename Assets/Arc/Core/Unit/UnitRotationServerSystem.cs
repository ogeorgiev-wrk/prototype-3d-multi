using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Arc.Core.Player;
using Arc.Core.Enemy;

namespace Arc.Core.Unit {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UnitRotationServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var unitRotationJob = new UnitRotationJob() { DeltaTime = SystemAPI.Time.DeltaTime };
            unitRotationJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }


    [BurstCompile]
    public partial struct UnitRotationJob : IJobEntity {
        public float DeltaTime;
        public void Execute(ref LocalTransform localTransform, in UnitMovementDirection direction, in UnitMovementTurnRate turnRate) {
            if (direction.Value.Equals(float3.zero)) return;

            var targetRotation = quaternion.LookRotation(direction.Value, math.up());
            localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, turnRate.Value * DeltaTime);
        }
    }

}
