using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Arc.Core.Camera {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct CameraInitializationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CameraInitializationFlag>();
        }

        public void OnUpdate(ref SystemState state) {
            if (CameraTargetSingleton.Instance == null) return;
            var cameraTargetTransform = CameraTargetSingleton.Instance.transform;

            foreach (var (playerCameraTarget, initFlag) in SystemAPI.Query<RefRW<CameraTarget>, EnabledRefRW<CameraInitializationFlag>>()) {
                playerCameraTarget.ValueRW.CameraTransform = cameraTargetTransform;
                initFlag.ValueRW = false;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct CameraMovementSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, cameraTarget) in SystemAPI.Query<LocalToWorld, CameraTarget>().WithAll<GhostOwnerIsLocal>().WithDisabled<CameraInitializationFlag>()) {
                cameraTarget.CameraTransform.Value.position = transform.Position;
            }
        }
    }
}
