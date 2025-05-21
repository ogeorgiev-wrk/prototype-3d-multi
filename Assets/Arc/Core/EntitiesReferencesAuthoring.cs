using Unity.Entities;
using UnityEngine;

namespace Arc.Core {
    public class EntitiesReferencesAuthoring : MonoBehaviour {
        public GameObject AttackPrefabGameObject;
    }

    public class EntitiesReferencesAuthoringBaker : Baker<EntitiesReferencesAuthoring> {
        public override void Bake(EntitiesReferencesAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences() {
                AttackPrefabEntity = GetEntity(authoring.AttackPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }

    public struct EntitiesReferences : IComponentData {
        public Entity AttackPrefabEntity;    
    }
}
