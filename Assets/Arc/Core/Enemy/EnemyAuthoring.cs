using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemyAuthoring : MonoBehaviour {
        public class Baker : Baker<EnemyAuthoring> {
            public override void Bake(EnemyAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyTargetPosition());
            }
        }
    }

    public struct EnemyTag : IComponentData { 
    }

    public struct EnemyTargetPosition : IComponentData {
        public float3 Value;
    }
}
