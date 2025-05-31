using Arc.Core.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemyTargetServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var targetQuery = SystemAPI.QueryBuilder().WithAll<PlayerTag, LocalTransform>().Build();
            if (targetQuery.IsEmpty) return;

            var targetArray = targetQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
            var enemyTargetJob = new EnemyTargetJob() { TargetArray = targetArray };
            enemyTargetJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyTargetJob : IJobEntity {
        [ReadOnly] public NativeArray<LocalTransform> TargetArray;
        public void Execute(ref EnemyState enemyState, in LocalTransform transform) {
            var closestPosition = TargetArray[0].Position;

            var minDistanceSq = math.distancesq(closestPosition, transform.Position);
            foreach (var target in TargetArray) {
                var currentDistanceSq = math.distancesq(target.Position, transform.Position);
                if (currentDistanceSq >= minDistanceSq) continue;

                minDistanceSq = currentDistanceSq;
                closestPosition = target.Position;
            }

            enemyState.TargetPosition = closestPosition;
            enemyState.DistanceFromTargetSq = minDistanceSq;
        }

    }
}
