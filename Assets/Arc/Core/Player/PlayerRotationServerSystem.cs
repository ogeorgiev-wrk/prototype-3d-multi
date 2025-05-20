using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using TMPro;
using UnityEngine;
using Unity.Transforms;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerRotationServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var playerRotationJob = new PlayerRotationJob();
            playerRotationJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(PlayerTag), typeof(Simulate))]
    public partial struct PlayerRotationJob : IJobEntity {
        public void Execute(ref UnitLookDirection lookDirection, in PlayerLookInput input) {
            lookDirection.Value = new float3(input.Value.x, 0, input.Value.y);
        }
    }
}
