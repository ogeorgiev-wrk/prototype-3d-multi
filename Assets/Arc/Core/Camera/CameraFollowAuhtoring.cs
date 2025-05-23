using Arc.Core.Player;
using Unity.Entities;
using UnityEngine;

namespace Arc.Core.Camera {
    public class CameraFollowAuhtoring : MonoBehaviour {

    }

    public class CameraFollowBaker : Baker<CameraFollowAuhtoring> {
        public override void Bake(CameraFollowAuhtoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraInitializationFlag());
            AddComponent(entity, new CameraTarget());
        }
    }

    public struct CameraInitializationFlag : IComponentData, IEnableableComponent { }
    public struct CameraTarget : IComponentData {
        public UnityObjectRef<Transform> CameraTransform;
    }
}
