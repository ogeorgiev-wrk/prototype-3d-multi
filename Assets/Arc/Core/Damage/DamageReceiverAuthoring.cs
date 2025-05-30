using Arc.Core.Unit;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Damage {
    public class DamageReceiverAuthoring : MonoBehaviour {
        public GameObject HealthContainerGameObject;
        public GameObject HealthBarGameObject;
        public int HealthMax = 100;

        public class Baker : Baker<DamageReceiverAuthoring> {
            public override void Bake(DamageReceiverAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageReceiverTag());
                AddComponent(entity, new DamageReceiverData() { HealthMax = authoring.HealthMax });
                AddComponent(entity, new DamageReceiverState() { HealthCurrent = authoring.HealthMax });
                AddComponent(entity, new DamageReceiverCollisionState());
                AddComponent(entity, new DamageReceiverCollisionTime());
                AddComponent(entity, new DamageReceiverUI() {
                    HealthContainerEntity = GetEntity(authoring.HealthContainerGameObject, TransformUsageFlags.Dynamic),
                    HealthBarEntity = GetEntity(authoring.HealthBarGameObject, TransformUsageFlags.NonUniformScale),
                });
                AddBuffer<DamageReceiverBuffer>(entity);
                AddComponent(entity, new DamageReceiverNoCollisionFlag());
                SetComponentEnabled<DamageReceiverNoCollisionFlag>(entity, false);
            }
        }
    }

    public struct DamageReceiverTag : IComponentData {

    }
    public struct DamageReceiverData : IComponentData {
        [GhostField] public int HealthMax;
    }

    public struct DamageReceiverState : IComponentData {
        [GhostField] public int HealthCurrent;
    }

    public struct DamageReceiverCollisionState : IComponentData {
        public CollisionState Value;
    }

    public struct DamageReceiverCollisionTime : IComponentData {
        public double Value;
    }

    public struct DamageReceiverUI : IComponentData {
        public Entity HealthContainerEntity;
        public Entity HealthBarEntity;
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
