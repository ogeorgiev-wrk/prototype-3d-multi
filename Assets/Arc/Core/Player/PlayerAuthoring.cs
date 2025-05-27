using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Player {
    public class PlayerAuthoring : MonoBehaviour {
        public Transform AttackOrigin;
        public PlayerAttackModifiers AttackModifiers;
        public List<GameObject> AttackPrefabGameObjectList;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerTag() { });
            AddComponent(entity, new PlayerMovementInput() { });
            AddComponent(entity, new PlayerTargetInput() { });
            AddComponent(entity, new PlayerAttackInput() { });
            AddComponent(entity, new PlayerWeaponSwapInput() { });
            AddComponent(entity, new PlayerAttackData() {
                Origin = authoring.AttackOrigin.localPosition,
            });
            AddComponent(entity, new PlayerAttackState() {
                Modifiers = authoring.AttackModifiers,
            });

            var buffer = AddBuffer<PlayerAttackPrefabBuffer>(entity);
            var prefabListLength = authoring.AttackPrefabGameObjectList.Count;
            for (int i = 0; i < prefabListLength; i++) {
                var currentEntity = GetEntity(authoring.AttackPrefabGameObjectList[i], TransformUsageFlags.Dynamic);
                buffer.Add(new PlayerAttackPrefabBuffer() { Value = currentEntity });
            }
        }
    }

    public struct PlayerTag : IComponentData { }
    public struct PlayerAttackData : IComponentData {
        public float3 Origin;
    }
    public struct PlayerAttackState : IComponentData {
        public PlayerAttackModifiers Modifiers;
    }

    public struct PlayerMovementInput : IInputComponentData {
        public float2 Value;
    }

    public struct PlayerTargetInput : IInputComponentData {
        public float3 Value;
    }

    public struct PlayerAttackInput : IInputComponentData {
        public InputEvent Value;
    }
    public struct PlayerWeaponSwapInput : IInputComponentData {
        public int Value;
    }
    public struct PlayerAttackPrefabBuffer : IBufferElementData {
        public Entity Value;
    }

    [Serializable]
    public struct PlayerAttackModifiers {
        public int Damage;
        public float MoveSpeed;
        public int MaxTargets;
        public float Lifetime;
        public float MaxDistance;
    }
}
