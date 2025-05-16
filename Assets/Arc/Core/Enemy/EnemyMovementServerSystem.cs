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
            var targetList = targetQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var enemyMovementJob = new EnemyMovementJob() { targetArray = targetList };
            enemyMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyMovementJob : IJobEntity {
        [ReadOnly] public NativeArray<LocalTransform> targetArray;
        public void Execute(ref UnitMovementDirection direction, in LocalTransform transform) {
            var targetPosition = targetArray[0].Position;

            var minDistanceSq = math.distancesq(targetPosition, transform.Position);
            foreach (var target in targetArray) {
                var currentDistanceSq = math.distancesq(target.Position, transform.Position);
                if (currentDistanceSq < minDistanceSq) {
                    minDistanceSq = currentDistanceSq;
                    targetPosition = target.Position;
                }
            }

            var targetDirectionBase = targetPosition - transform.Position;
            var targetDirectionNormalized = math.normalize(targetDirectionBase);

            direction.Value = targetDirectionNormalized;
        }
    }
}
