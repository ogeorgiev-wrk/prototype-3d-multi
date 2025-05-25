using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core {
    public class EntitiesReferencesAuthoring : MonoBehaviour {
        public Color InactiveColor;
        public Color ActiveColor;
        public GameObject AttackPrefabGameObject;
    }

    public class EntitiesReferencesAuthoringBaker : Baker<EntitiesReferencesAuthoring> {
        public override void Bake(EntitiesReferencesAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences() {
                AttackPrefabEntity = GetEntity(authoring.AttackPrefabGameObject, TransformUsageFlags.Dynamic),
                InactiveColor = (Vector4)authoring.InactiveColor,
                ActiveColor = (Vector4)authoring.ActiveColor,
            });
        }
    }

    public struct EntitiesReferences : IComponentData {
        public Entity AttackPrefabEntity;
        public float4 InactiveColor;
        public float4 ActiveColor;
    }
}
