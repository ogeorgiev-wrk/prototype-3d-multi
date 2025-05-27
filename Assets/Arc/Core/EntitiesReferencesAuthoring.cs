using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Arc.Core {
    public class EntitiesReferencesAuthoring : MonoBehaviour {
        public class Baker : Baker<EntitiesReferencesAuthoring> {
            public override void Bake(EntitiesReferencesAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

            }
        }
    }

    
    
}
