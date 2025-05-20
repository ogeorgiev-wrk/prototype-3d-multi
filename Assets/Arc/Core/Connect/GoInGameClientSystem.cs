using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Connect {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    partial struct GoInGameClientSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkId>();

            var networkQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(networkQuery));
            networkQuery.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach(var (networkId, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess()) {
                ecb.AddComponent<NetworkStreamInGame>(entity);
                //Debug.Log("[GoInGameClientSystem]Connecting: " + entity + " :: " + networkId.ValueRO.Value);

                var rpcEntity = ecb.CreateEntity();
                ecb.AddComponent(rpcEntity, new GoInGameRequestRpc() { SpawnerIndex = 0 });
                ecb.AddComponent(rpcEntity, new SendRpcCommandRequest());
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    public struct GoInGameRequestRpc : IRpcCommand {
        public int SpawnerIndex;
    }
}
