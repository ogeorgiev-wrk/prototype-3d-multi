using Unity.NetCode;
using UnityEngine;

namespace Arc.Core {
    [UnityEngine.Scripting.Preserve]
    public class GameBootstrap : ClientServerBootstrap {
        public override bool Initialize(string defaultWorldName) {
            Debug.Log("[GameBootstrap]Initialize");
            AutoConnectPort = 7979;
            return base.Initialize(defaultWorldName);
        }
    }
}
