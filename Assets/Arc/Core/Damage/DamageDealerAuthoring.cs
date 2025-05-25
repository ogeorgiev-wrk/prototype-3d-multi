using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageDealerAuthoring : MonoBehaviour {
        public class Baker : Baker<DamageDealerAuthoring> {
            public override void Bake(DamageDealerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageDealerTag());
                AddComponent(entity, new DamageDealerData());
                AddComponent(entity, new DamageDealerState());
                AddBuffer<DamageDealerBuffer>(entity);
                AddComponent(entity, new DamageDealerDestroyFlag());
                SetComponentEnabled<DamageDealerDestroyFlag>(entity, false);
            }
        }
    }

    public struct DamageDealerTag : IComponentData {

    }

    public struct DamageDealerData : IComponentData {
        public float3 StartPosition;
        public float3 Direction;
        public float MaxDistanceSq;
        public float MoveSpeed;
        public int MaxTargets;
        public int Damage;
    }

    public struct DamageDealerState : IComponentData {
        public float DistanceCurrentSq;
    }

    public struct DamageDealerBuffer : IBufferElementData {
        public int Value;
    }

    public struct DamageDealerDestroyFlag : IComponentData, IEnableableComponent {

    }
}
