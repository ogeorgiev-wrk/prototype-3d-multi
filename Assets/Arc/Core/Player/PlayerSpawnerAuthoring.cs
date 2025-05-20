using Arc.Core.Enemy;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Player {
    public class PlayerSpawnerAuthoring : MonoBehaviour {
        public GameObject PrefabGameObject;
        
        public class Baker : Baker<PlayerSpawnerAuthoring> {
            public override void Bake(PlayerSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerSpawnerTag());
                AddComponent(entity, new PlayerSpawnerData() {
                    PrefabEntity = GetEntity(authoring.PrefabGameObject, TransformUsageFlags.Dynamic),
                });
            }
        }
    }

    public struct PlayerSpawnerTag : IComponentData {

    }

    public struct PlayerSpawnerData : IComponentData {
        public Entity PrefabEntity;
    }
}
