using System;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;
using Unity.NetCode;

namespace Arc.Core.Unit {
    public struct HealthBarState : IComponentData {
        public bool IsVisible;
        public float Progress;
    }

    public class HealthBarAuthoring : MonoBehaviour {
        public class Baker : Baker<HealthBarAuthoring> {
            public override void Bake(HealthBarAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new HealthBarState() {
                    IsVisible = false,
                    Progress = 1.0f,
                });
            }
        }
    }
}
