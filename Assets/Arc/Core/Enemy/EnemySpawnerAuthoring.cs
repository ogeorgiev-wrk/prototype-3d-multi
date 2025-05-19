using Arc.Core.Enemy;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemySpawnerAuthoring : MonoBehaviour {
        public GameObject EnemyPrefabGameObject;
        public float SpawnRate;
        public float SpawnRadiusMin;
        public float SpawnRadiusMax;
        public uint SpawnSeed;
        
        public class Baker : Baker<EnemySpawnerAuthoring> {
            public override void Bake(EnemySpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemySpawnerTag());
                AddComponent(entity, new EnemySpawnerState() {
                    Random = new Unity.Mathematics.Random(authoring.SpawnSeed)
                });
                AddComponent(entity, new EnemySpawnerData() {
                    EnemyPrefabEntity = GetEntity(authoring.EnemyPrefabGameObject, TransformUsageFlags.Dynamic),
                    SpawnRate = authoring.SpawnRate,
                    SpawnRadiusMin = authoring.SpawnRadiusMin,
                    SpawnRadiusMax = authoring.SpawnRadiusMax,
                });
            }
        }
    }

    public struct EnemySpawnerTag : IComponentData {
    }

    public struct EnemySpawnerData : IComponentData {
        public Entity EnemyPrefabEntity;
        public float SpawnRate;
        public float SpawnRadiusMin;
        public float SpawnRadiusMax;
    }

    public struct EnemySpawnerState : IComponentData {
        public float Cooldown;
        public Unity.Mathematics.Random Random;
    }
}
