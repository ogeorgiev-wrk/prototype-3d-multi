using Arc.Core.Player;
using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemyMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var targetQuery = SystemAPI.QueryBuilder().WithAll<PlayerTag, LocalTransform>().Build();
            if (targetQuery.IsEmpty) return;

            var targetArray = targetQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
            var enemyMovementJob = new EnemyMovementJob() { TargetArray = targetArray };
            enemyMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyMovementJob : IJobEntity {
        [ReadOnly] public NativeArray<LocalTransform> TargetArray;
        public void Execute(ref UnitMovementDirection movementDirection, ref UnitLookDirection lookDirection, in LocalTransform transform) {
            var targetPosition = TargetArray[0].Position;

            var minDistanceSq = math.distancesq(targetPosition, transform.Position);
            foreach (var target in TargetArray) {
                var currentDistanceSq = math.distancesq(target.Position, transform.Position);
                if (currentDistanceSq >= minDistanceSq) continue;

                minDistanceSq = currentDistanceSq;
                targetPosition = target.Position;
            }

            var targetDirectionBase = targetPosition - transform.Position;
            var targetDirectionNormalized = math.normalize(targetDirectionBase);

            movementDirection.Value = targetDirectionNormalized;
            lookDirection.Value = targetDirectionNormalized;
        }
    }
}
