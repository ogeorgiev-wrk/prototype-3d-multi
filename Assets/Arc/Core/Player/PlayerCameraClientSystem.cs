using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PlayerCameraInitializationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PlayerCameraInitializationFlag>();
        }

        public void OnUpdate(ref SystemState state) {
            if (PlayerCameraTargetSingleton.Instance == null) return;
            var cameraTargetTransform = PlayerCameraTargetSingleton.Instance.transform;

            foreach (var (playerCameraTarget, initFlag) in SystemAPI.Query<RefRW<PlayerCameraTarget>, EnabledRefRW<PlayerCameraInitializationFlag>>()) {
                playerCameraTarget.ValueRW.CameraTransform = cameraTargetTransform;
                initFlag.ValueRW = false;
            }
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct PlayerCameraMoveSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, cameraTarget) in SystemAPI.Query<LocalToWorld, PlayerCameraTarget>().WithAll<PlayerTag>().WithDisabled<PlayerCameraInitializationFlag>()) {
                cameraTarget.CameraTransform.Value.position = transform.Position;
            }
        }
    }
}
