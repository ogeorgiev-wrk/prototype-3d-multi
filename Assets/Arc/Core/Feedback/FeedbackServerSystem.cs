using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Arc.Core.Damage;

namespace Arc.Core.Feedback {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct FeedbackServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (shaderState, collisionState) in SystemAPI.Query<RefRW<FeedbackShaderStateOverride>, RefRO<DamageReceiverCollisionState>>()) {
                bool isCollision = collisionState.ValueRO.Value != CollisionState.None;
                shaderState.ValueRW.Value = isCollision ? 1 : 0;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
