using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageDealerAuthoring : MonoBehaviour {
        public DamageDealerSource Source;
        public DamageDealerParams Params;

        public class Baker : Baker<DamageDealerAuthoring> {
            public override void Bake(DamageDealerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageDealerTag());
                AddComponent(entity, new DamageDealerData());
                AddComponent(entity, new DamageDealerState());
                AddComponent(entity, new DamageDealerSetup() {
                    Source = authoring.Source,
                    BaseParams = authoring.Params,
                });
                AddBuffer<DamageDealerBuffer>(entity);
                AddComponent(entity, new DamageDealerNoCollisionFlag());
                SetComponentEnabled<DamageDealerNoCollisionFlag>(entity, false);
            }
        }
    }

    public struct DamageDealerTag : IComponentData {

    }

    public struct DamageDealerSetup : IComponentData {
        public DamageDealerSource Source;
        public DamageDealerParams BaseParams;
    }

    public struct DamageDealerData : IComponentData {
        public float3 StartPosition;
        public float3 Direction;
        public DamageDealerParams ModifiedParams;
    }

    public struct DamageDealerState : IComponentData {
        public float3 PositionCurrent;
        public float DistanceSqCurrent;
        public float LifetimeCurrent;
    }

    public struct DamageDealerBuffer : IBufferElementData {
        public int Value;
    }

    public struct DamageDealerNoCollisionFlag : IComponentData, IEnableableComponent {

    }

    [Serializable]
    public enum DamageDealerSource {
        NONE = 0,
        ORIGIN = 1,
        TARGET = 2
    }

    [Serializable]
    public struct DamageDealerParams {
        public int Damage;
        public float MoveSpeed;
        public int MaxTargets;
        public float MaxDistance;
        public float MaxLifetime;
    }
}
