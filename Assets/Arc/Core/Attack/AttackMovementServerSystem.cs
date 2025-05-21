using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Arc.Core.Attack {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct AttackMovementServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var movementJob = new AttackMovementJob();
            movementJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct AttackMovementJob : IJobEntity {
        public void Execute(ref PhysicsVelocity physicsVelocity, ref AttackState attackState, in AttackData attackData, in LocalTransform transform) {
            if (attackState.Direction.Equals(float3.zero)) return;
            if (attackData.MoveSpeed == 0f) return;
            if (attackState.IsMaxDistance) return;

            physicsVelocity.Angular = float3.zero;

            var currentDistanceSq = math.distancesq(transform.Position, attackState.StartPosition);
            if (currentDistanceSq >= math.square(attackData.Range)) {
                attackState.IsMaxDistance = true;
                physicsVelocity.Linear = float3.zero;
                return;
            }

            float3 moveDirectionNormalized = math.normalize(attackState.Direction);
            physicsVelocity.Linear = moveDirectionNormalized * attackData.MoveSpeed;
        }
    }
}
