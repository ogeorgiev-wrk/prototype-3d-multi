using Unity.Entities;
using UnityEngine;

namespace Arc.Core.Enemy {
    public class EnemyAuthoring : MonoBehaviour {

    }

    public class EnemyAuthoringBaker : Baker<EnemyAuthoring> {
        public override void Bake(EnemyAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTag());
        }
    }

    public struct EnemyTag : IComponentData { }
}
