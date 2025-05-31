using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemyMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var enemyMovementJob = new EnemyMovementJob();
            enemyMovementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyMovementJob : IJobEntity {
        public void Execute(ref UnitMovementDirection movementDirection, ref UnitLookDirection lookDirection, in EnemyState enemyState, in EnemyData enemyData, in LocalTransform transform) {
            var distanceSq = math.distancesq(enemyState.TargetPosition, transform.Position);
            bool isWithinRange = distanceSq <= enemyData.AttackRangeSq;

            var targetDirectionBase = enemyState.TargetPosition - transform.Position;
            var targetDirectionNormalized = math.normalize(targetDirectionBase);

            movementDirection.Value = isWithinRange ? float3.zero : targetDirectionNormalized;
            lookDirection.Value = targetDirectionNormalized;
        }
    }
}
