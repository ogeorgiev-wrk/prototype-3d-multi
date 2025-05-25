using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageDealerAuthoring : MonoBehaviour {
        public float MaxDistance;
        public int MaxTargets = 1;
        public float MoveSpeed;        
        public int Damage;

        public class Baker : Baker<DamageDealerAuthoring> {
            public override void Bake(DamageDealerAuthoring authoring) {
                if (authoring.MaxTargets <= 0) {
                    throw new System.ArgumentException($"{GetName()}: {nameof(MaxTargets)} is 0.");
                }
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageDealerTag());
                AddComponent(entity, new DamageDealerData() {
                    MaxDistanceSq = math.square(authoring.MaxDistance),
                    MoveSpeed = authoring.MoveSpeed,
                    MaxTargets = authoring.MaxTargets,
                    Damage = authoring.Damage,
                });
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
