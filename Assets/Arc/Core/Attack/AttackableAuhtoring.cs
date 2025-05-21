using Arc.Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Arc.Core.Attack {
    public class AttackableAuhtoring : MonoBehaviour {
        public int HealthMax;

        public class Baker : Baker<AttackableAuhtoring> {
            public override void Bake(AttackableAuhtoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new AttackableTag());
                AddComponent(entity, new AttackableHealthCurrent());
                AddComponent(entity, new AttackableHealthMax() { Value = authoring.HealthMax });
            }
        }
    }

    public struct AttackableTag : IComponentData {

    }

    public struct AttackableHealthCurrent : IComponentData {
        public int Value;
    }

    public struct AttackableHealthMax : IComponentData {
        public int Value;
    }
}
