using Arc.Core.Unit;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageReceiverAuthoring : MonoBehaviour {
        public int HealthMax;

        public class Baker : Baker<DamageReceiverAuthoring> {
            public override void Bake(DamageReceiverAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageReceiverTag());
                AddComponent(entity, new DamageReceiverData() { HealthMax = authoring.HealthMax });
                AddComponent(entity, new DamageReceiverState() { HealthCurrent = authoring.HealthMax });
                AddBuffer<DamageReceiverBuffer>(entity);
                AddComponent(entity, new DamageReceiverDestroyFlag());
                SetComponentEnabled<DamageReceiverDestroyFlag>(entity, false);
            }
        }
    }

    public struct DamageReceiverTag : IComponentData {

    }
    public struct DamageReceiverData : IComponentData {
        public int HealthMax;
    }

    public struct DamageReceiverState : IComponentData {
        public int HealthCurrent;
    }

    public struct DamageReceiverBuffer : IBufferElementData {
        public int Value;
    }

    public struct DamageReceiverDestroyFlag : IComponentData, IEnableableComponent {

    }
}
