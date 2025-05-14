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
        }
    }

    public struct PlayerTag : IComponentData { }
    public struct PlayerInput : IInputComponentData {
        public float2 MovementInput;
        public InputEvent AttackInput;
    }
}
