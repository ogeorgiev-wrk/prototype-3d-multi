using Unity.Entities;
using UnityEngine;

namespace Arc.Core {
    public class EntitiesReferencesAuthoring : MonoBehaviour {
        public GameObject PlayerPrefabGameObject;
        public GameObject EnemyPrefabGameObject;
    }

    public class EntitiesReferencesAuthoringBaker : Baker<EntitiesReferencesAuthoring> {
        public override void Bake(EntitiesReferencesAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences() {
                PlayerPrefabEntity = GetEntity(authoring.PlayerPrefabGameObject, TransformUsageFlags.Dynamic),
                EnemyPrefabEntity = GetEntity(authoring.EnemyPrefabGameObject, TransformUsageFlags.Dynamic),
            });
        }
    }

    public struct EntitiesReferences : IComponentData {
        public Entity PlayerPrefabEntity;
        public Entity EnemyPrefabEntity;
    
    }
}
