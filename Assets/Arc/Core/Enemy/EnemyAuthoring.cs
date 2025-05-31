using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemyAuthoring : MonoBehaviour {
        public GameObject AttackGameObjectPrefab;
        public Transform AttackOrigin;
        public float AttackDistanceMax;
        public float AttackCooldownMax;

        public class Baker : Baker<EnemyAuthoring> {
            public override void Bake(EnemyAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
                AddComponent(entity, new EnemyAttackState());
                AddComponent(entity, new EnemyAttackData() {
                    EntityPrefab = GetEntity(authoring.AttackGameObjectPrefab, TransformUsageFlags.Dynamic),
                    Origin = authoring.AttackOrigin.localPosition,
                    DistanceFromTargetMax = math.square(authoring.AttackDistanceMax),
                    CooldownMax = authoring.AttackCooldownMax,
                    
                });
            }
        }
    }

    public struct EnemyTag : IComponentData { 

    }

    public struct EnemyAttackData : IComponentData {
        public Entity EntityPrefab;
        public float3 Origin;
        public float DistanceFromTargetMax;
        public float CooldownMax;
        
    }

    public struct EnemyAttackState : IComponentData {
        public float3 TargetPosition;
        public float DistanceFromTargetSq;
        public float CooldownCurrent;
    }
}
