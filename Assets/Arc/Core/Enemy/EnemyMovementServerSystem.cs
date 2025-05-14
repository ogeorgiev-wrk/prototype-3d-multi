using Arc.Core.Player;
using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Arc.Core.Enemy {
    partial struct EnemyMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var enemyMovementJob = new EnemyMovementJob() { };
            enemyMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct EnemyMovementJob : IJobEntity {
        public void Execute(ref UnitMovementDirection direction, in LocalTransform transform) {
            var targetDirectionBase = float3.zero - transform.Position;
            var targetDirectionNormalized = math.normalize(targetDirectionBase);

            direction.Value = targetDirectionNormalized;
        }
    }
}
