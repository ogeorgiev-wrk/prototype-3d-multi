using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arc.Core.Player {
    public class PlayerAuthoring : MonoBehaviour {

    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag() { });
            AddComponent(entity, new PlayerCameraInitializationFlag());
            AddComponent(entity, new PlayerCameraTarget());
            AddComponent(entity, new PlayerMovementInput() { });
            AddComponent(entity, new PlayerLookInput() { });
            AddComponent(entity, new PlayerAttackInput() { });
        }
    }

    public struct PlayerTag : IComponentData { }

    public struct PlayerMovementInput : IInputComponentData {
        public float2 Value;
    }

    public struct PlayerLookInput : IInputComponentData {
        public float2 Value;
    }

    public struct PlayerAttackInput : IInputComponentData {
        public InputEvent PrimaryAttack;
        public InputEvent SecondaryAttack;
    }
    public struct PlayerCameraInitializationFlag : IComponentData, IEnableableComponent { }
    public struct PlayerCameraTarget : IComponentData {
        public UnityObjectRef<Transform> CameraTransform;
    }
}
