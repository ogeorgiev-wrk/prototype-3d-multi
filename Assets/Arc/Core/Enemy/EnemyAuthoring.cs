using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemyAuthoring : MonoBehaviour {
        public GameObject MeshGameObject;
        public class Baker : Baker<EnemyAuthoring> {
            public override void Bake(EnemyAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyTargetPosition());
                AddComponent(entity, new MaterialColorData());
                AddComponent(entity, new EnemyMesh() {
                    MeshEntity = GetEntity(authoring.MeshGameObject, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct EnemyTag : IComponentData { 
    }

    public struct EnemyTargetPosition : IComponentData {
        public float3 Value;
    }

    public struct MaterialColorData : IComponentData {
        [GhostField] public float4 Value; // RGBA Color
    }

    public struct EnemyMesh : IComponentData {
        public Entity MeshEntity;
    }
}
