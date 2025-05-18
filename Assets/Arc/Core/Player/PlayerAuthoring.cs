using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Player {
    public class PlayerAuthoring : MonoBehaviour {

    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag() { });
            AddComponent(entity, new PlayerInput() { });
            AddComponent(entity, new PlayerCameraInitializationFlag());
            AddComponent(entity, new PlayerCameraTarget());
        }
    }

    public struct PlayerTag : IComponentData { }
    public struct PlayerInput : IInputComponentData {
        public float2 MovementInput;
        public float2 LookInput;
        public InputEvent AttackInput;
    }
    public struct PlayerCameraInitializationFlag : IComponentData, IEnableableComponent { }
    public struct PlayerCameraTarget : IComponentData {
        public UnityObjectRef<Transform> CameraTransform;
    }
}
