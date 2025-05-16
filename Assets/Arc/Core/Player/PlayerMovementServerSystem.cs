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
            foreach (var (movementDirection, playerInput) in SystemAPI.Query<RefRW<UnitMovementDirection>, RefRO<PlayerInput>>().WithAll<Simulate>()) {
                var inputVector = playerInput.ValueRO.MovementInput;
                movementDirection.ValueRW.Value = new float3(inputVector.x, 0, inputVector.y);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
