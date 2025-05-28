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
            var deltaTime = SystemAPI.Time.DeltaTime;

            var lifecycleJob = new FeedbackLifecycleJob() { DeltaTime = deltaTime };
            lifecycleJob.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    public partial struct FeedbackLifecycleJob : IJobEntity {
        public float DeltaTime;
        public void Execute(ref FeedbackState feedbackState, ref FeedbackShaderStateOverride shaderState, in FeedbackSetup feedbackSetup, in DynamicBuffer<DamageReceiverBuffer> receiverBuffer) {
            if (receiverBuffer.Length > 0) {
                feedbackState.LifetimeCurrent = feedbackSetup.LifetimeMax;
            }
            
            feedbackState.LifetimeCurrent -= DeltaTime;

            bool hasFeedback = feedbackState.LifetimeCurrent > 0;
            shaderState.Value = hasFeedback ? 1 : 0;
        }
    }
}
