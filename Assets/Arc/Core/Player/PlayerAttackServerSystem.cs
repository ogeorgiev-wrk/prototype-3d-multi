using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Transforms;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerAttackServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            foreach (var (localTransform, playerInput) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerAttackInput>>().WithAll<Simulate>()) {
                if (!networkTime.IsFinalFullPredictionTick) continue;

                var isPrimary = playerInput.ValueRO.PrimaryAttack.IsSet;
                var isSecondary = playerInput.ValueRO.SecondaryAttack.IsSet;
                if (!isPrimary && !isSecondary) {
                    localTransform.ValueRW.Scale = 1f;
                    continue;
                }
                localTransform.ValueRW.Scale = isPrimary ? 1.2f : .8f;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
