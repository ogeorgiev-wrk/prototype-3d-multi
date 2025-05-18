using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Arc.Core.Unit {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UnitInitServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var unitInitJob = new UnitInitJob();
            unitInitJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct UnitInitJob : IJobEntity {
        public void Execute(RefRW<PhysicsMass> physicsMass, EnabledRefRW<UnitInitFlag> initFlag) {
            physicsMass.ValueRW.InverseInertia = float3.zero;
            initFlag.ValueRW = false;
        }
    }
}
