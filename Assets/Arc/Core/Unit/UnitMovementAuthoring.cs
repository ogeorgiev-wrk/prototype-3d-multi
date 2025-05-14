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

    public struct UnitMovementTurnRate : IComponentData { 
        public float Value; 
    }

    public struct UnitMovementInitFlag : IComponentData, IEnableableComponent { }

    public class UnitMovementAuthoring : MonoBehaviour {
        public float MovementSpeed;
        public float TurnRate;
    }

    public class UnitMovementBaker : Baker<UnitMovementAuthoring> {
        public override void Bake(UnitMovementAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitMovementInitFlag());
            AddComponent(entity, new UnitMovementSpeed() { Value = authoring.MovementSpeed });
            AddComponent(entity, new UnitMovementDirection());
            AddComponent(entity, new UnitMovementTurnRate() { Value = authoring.TurnRate });
        }
    }
}
