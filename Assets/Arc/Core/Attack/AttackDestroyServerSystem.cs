using Arc.Core.Unit;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

namespace Arc.Core.Attack {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct AttackDestroyServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (attackState, entity) in SystemAPI.Query<RefRO<AttackState>>().WithEntityAccess()) {
                if (!attackState.ValueRO.ShouldDestroy) continue;
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
