using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var playerMovementJob = new PlayerMovementJob();
            playerMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(PlayerTag), typeof(Simulate))]
    public partial struct PlayerMovementJob : IJobEntity {
        public void Execute(ref UnitMovementDirection movementDirection, in PlayerMovementInput input) {
            movementDirection.Value = new float3(input.Value.x, 0, input.Value.y);
        }
    }
}
