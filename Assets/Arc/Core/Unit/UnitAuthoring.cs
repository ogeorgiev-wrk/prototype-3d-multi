using System;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;
using Unity.NetCode;

namespace Arc.Core.Unit {
    public struct UnitMovementSpeed : IComponentData {
        public float Value;
    }
    public struct UnitMovementTurnRate : IComponentData {
        public float Value;
    }

    public struct UnitMovementDirection : IComponentData {
        public float3 Value;
    }
    public struct UnitLookDirection : IComponentData {
        public float3 Value;
    }

    public struct UnitInitFlag : IComponentData, IEnableableComponent { }

    public class UnitAuthoring : MonoBehaviour {
        [Header("MOVEMENT")]
        public float MovementSpeed;
        public float TurnRate;

        public class Baker : Baker<UnitAuthoring> {
            public override void Bake(UnitAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitInitFlag());
                AddComponent(entity, new UnitMovementSpeed() { Value = authoring.MovementSpeed });
                AddComponent(entity, new UnitMovementTurnRate() { Value = authoring.TurnRate });
                AddComponent(entity, new UnitMovementDirection());
                AddComponent(entity, new UnitLookDirection());
            }
        }
    }
}
