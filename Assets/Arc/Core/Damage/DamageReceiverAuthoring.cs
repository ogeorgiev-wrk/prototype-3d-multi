using Arc.Core.Unit;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageReceiverAuthoring : MonoBehaviour {
        public int HealthMax = 100;

        public class Baker : Baker<DamageReceiverAuthoring> {
            public override void Bake(DamageReceiverAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageReceiverTag());
                AddComponent(entity, new DamageReceiverData() { HealthMax = authoring.HealthMax });
                AddComponent(entity, new DamageReceiverState() { HealthCurrent = authoring.HealthMax });
                AddComponent(entity, new DamageReceiverCollisionState());
                AddComponent(entity, new DamageReceiverCollisionTime());
                AddBuffer<DamageReceiverBuffer>(entity);
                AddComponent(entity, new DamageReceiverNoCollisionFlag());
                SetComponentEnabled<DamageReceiverNoCollisionFlag>(entity, false);
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

    public struct DamageReceiverCollisionState : IComponentData {
        public CollisionState Value;
    }

    public struct DamageReceiverCollisionTime : IComponentData {
        public double Value;
    }

    public struct DamageReceiverBuffer : IBufferElementData {
        public int Value;
    }

    public struct DamageReceiverNoCollisionFlag : IComponentData, IEnableableComponent {

    }

    public enum CollisionState {
        None = 0,
        Enter = 1,
        Inside = 2,
        Exit = 3
    }
}
