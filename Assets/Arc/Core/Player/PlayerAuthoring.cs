using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Player {
    public class PlayerAuthoring : MonoBehaviour {
        public Transform AttackOrigin;
        public float AttackRate;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag() { });
            AddComponent(entity, new PlayerMovementInput() { });
            AddComponent(entity, new PlayerLookInput() { });
            AddComponent(entity, new PlayerAttackInput() { });
            AddComponent(entity, new PlayerAttackData() {
                Origin = authoring.AttackOrigin.localPosition,
                AttackRate = authoring.AttackRate,
            });
            AddComponent(entity, new PlayerAttackState());
        }
    }

    public struct PlayerTag : IComponentData { }
    public struct PlayerAttackData : IComponentData {
        public float3 Origin;
        public float AttackRate;
    }
    public struct PlayerAttackState : IComponentData {
        public float Cooldown;
    }

    public struct PlayerMovementInput : IInputComponentData {
        public float2 Value;
    }

    public struct PlayerLookInput : IInputComponentData {
        public float3 Value;
    }

    public struct PlayerAttackInput : IInputComponentData {
        public InputEvent Value;
    }
}
