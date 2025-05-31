using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;

namespace Arc.Core.Damage {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    partial struct DamageReceiverLifecycleClientSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var ecbParallel = ecb.AsParallelWriter();
            var cameraForward = Vector3.zero;
            var cameraUp = Vector3.zero;
            if (Camera.main != null) {
                cameraForward = Camera.main.transform.forward;
                cameraUp = Camera.main.transform.up;
            }

            var healthBarJob = new DamageReceiverHealthBarJob() {
                EcbParallel = ecbParallel,
                CameraForward = cameraForward,
                CameraUp = cameraUp,
                PostTransformMatrixLookup = SystemAPI.GetComponentLookup<PostTransformMatrix>(false),
                LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false)
            };
            healthBarJob.Schedule();
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    [BurstCompile]
    [WithAll(typeof(DamageReceiverTag))]
    public partial struct DamageReceiverHealthBarJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter EcbParallel;
        public float3 CameraForward;
        public float3 CameraUp;
        public ComponentLookup<PostTransformMatrix> PostTransformMatrixLookup;
        public ComponentLookup<LocalTransform> LocalTransformLookup;

        public void Execute([ChunkIndexInQuery] int sortKey, ref DamageReceiverUI receiverUI, in DamageReceiverState receiverState, in DamageReceiverData receiverData, in Entity entity) {
            var healthNormalized = (float)receiverState.HealthCurrent / receiverData.HealthMax;

            var parentTransform = LocalTransformLookup[entity];
            var barTransform = LocalTransformLookup[receiverUI.HealthContainerEntity];
            barTransform.Rotation = parentTransform.InverseTransformRotation(quaternion.LookRotation(CameraForward, CameraUp));
            LocalTransformLookup[receiverUI.HealthContainerEntity] = barTransform;

            var transformMatrix = PostTransformMatrixLookup[receiverUI.HealthBarEntity];
            transformMatrix.Value = float4x4.Scale(healthNormalized, 1f, 1f);
            PostTransformMatrixLookup[receiverUI.HealthBarEntity] = transformMatrix;
        }
    }
}
