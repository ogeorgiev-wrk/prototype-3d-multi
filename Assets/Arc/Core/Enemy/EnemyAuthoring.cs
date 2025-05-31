using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemyAuthoring : MonoBehaviour {
        public float AttackRange;
        public float AttackCooldown;
        public GameObject AttackPrefab;

        public class Baker : Baker<EnemyAuthoring> {
            public override void Bake(EnemyAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyState());
                AddComponent(entity, new EnemyData() {
                    AttackRangeSq = math.square(authoring.AttackRange),
                    AttackCooldownMax = authoring.AttackCooldown,
                    AttackEntity = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct EnemyTag : IComponentData { 

    }

    public struct EnemyData : IComponentData {
        public float AttackRangeSq;
        public float AttackCooldownMax;
        public Entity AttackEntity;
    }

    public struct EnemyState : IComponentData {
        public float3 TargetPosition;
        public float DistanceFromTargetSq;
        public float AttackCooldownCurrent;
    }
}
