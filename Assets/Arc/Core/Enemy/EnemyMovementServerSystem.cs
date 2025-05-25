using Arc.Core.Unit;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Arc.Core.Damage;
using Unity.Rendering;

namespace Arc.Core.Enemy {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    partial struct UpdateMaterialColorSystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {

            foreach (var (meshContainer, colorData) in SystemAPI.Query<RefRW<EnemyMesh>, RefRO<MaterialColorData>>()) {
                var meshColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(meshContainer.ValueRO.MeshEntity);
                meshColor.ValueRW.Value = colorData.ValueRO.Value;
            }

        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct EnemyMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (var (meshColor, collisionState) in SystemAPI.Query<RefRW<MaterialColorData>, RefRO<DamageReceiverCollisionState>>()) {
                bool isCollision = collisionState.ValueRO.Value != CollisionState.None;
                meshColor.ValueRW.Value = isCollision ? entitiesReferences.ActiveColor : entitiesReferences.InactiveColor;
            }

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
        public void Execute(ref UnitMovementDirection movementDirection, ref UnitLookDirection lookDirection, in EnemyTargetPosition targetPosition, in LocalTransform transform) {
            var targetDirectionBase = targetPosition.Value - transform.Position;
            var targetDirectionNormalized = math.normalize(targetDirectionBase);

            movementDirection.Value = targetDirectionNormalized;
            lookDirection.Value = targetDirectionNormalized;
        }
    }
}
