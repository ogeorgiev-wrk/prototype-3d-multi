using System;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Arc.Core.Enemy;
using Unity.NetCode;
using Unity.Rendering;

namespace Arc.Core.Feedback {

    public class FeedbackAuthoring : MonoBehaviour {
        public float LifetimeMax = 0.1f;
        public class Baker : Baker<FeedbackAuthoring> {
            public override void Bake(FeedbackAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FeedbackShaderStateOverride());
                AddComponent(entity, new FeedbackState());
                AddComponent(entity, new FeedbackSetup() {
                    LifetimeMax = authoring.LifetimeMax,
                });
            }
        }
    }

    [MaterialProperty("_FeedbackState")]
    public struct FeedbackShaderStateOverride : IComponentData {
        [GhostField] public float Value;
    }

    public struct FeedbackState : IComponentData {
        public float LifetimeCurrent;
    }

    public struct FeedbackSetup : IComponentData {
        public float LifetimeMax;
    }
}
