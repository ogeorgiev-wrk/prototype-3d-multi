using System;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;
using Unity.NetCode;

namespace Arc.Core.Unit {
    public struct UnitMovementSpeed : IComponentData {
        public float Value;
    }

    public struct UnitMovementDirection : IComponentData {
        public float3 Value;
    }

    public class UnitMovementAuthoring : MonoBehaviour {
        public float MovementSpeed;
    }

    public class UnitMovementBaker : Baker<UnitMovementAuthoring> {
        public override void Bake(UnitMovementAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitMovementSpeed() { Value = authoring.MovementSpeed });
            AddComponent(entity, new UnitMovementDirection());
        }
    }
}
