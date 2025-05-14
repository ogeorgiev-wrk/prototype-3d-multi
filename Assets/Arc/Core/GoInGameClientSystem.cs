using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    partial struct GoInGameClientSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<NetworkId>();
            var eqb = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(eqb));
            eqb.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach(var (networkId, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess()) {
                ecb.AddComponent<NetworkStreamInGame>(entity);
                Debug.Log("[GoInGameClientSystem]Connecting: " + entity + " :: " + networkId.ValueRO.Value);

                var rpcEntity = ecb.CreateEntity();
                ecb.AddComponent<GoInGameRequestRpc>(rpcEntity);
                ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
