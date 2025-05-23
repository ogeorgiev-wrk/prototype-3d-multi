using Arc.Core.Unit;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Attack {
    public class AttackAuthoring : MonoBehaviour {
        public GameObject PrefabGameObject;
        public float Range;
        public float MoveSpeed;
        public int Damage;

        public class Baker : Baker<AttackAuthoring> {
            public override void Bake(AttackAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AttackTag());
                AddComponent(entity, new AttackData() {
                    PrefabEntity = GetEntity(authoring.PrefabGameObject, TransformUsageFlags.Dynamic),
                    Range = authoring.Range,
                    MoveSpeed = authoring.MoveSpeed,
                    Damage = authoring.Damage,
                });
                AddComponent(entity, new AttackState());
            }
        }
    }

    public struct AttackTag : IComponentData {

    }

    public struct AttackData : IComponentData {
        public Entity PrefabEntity;
        public float Range;
        public float MoveSpeed;
        public int Damage;
    }

    public struct AttackState : IComponentData {
        public float3 StartPosition;
        public float3 Direction;
        public bool ShouldDestroy;
    }
}
