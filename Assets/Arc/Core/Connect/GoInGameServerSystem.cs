using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Arc.Core.Connect {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct GoInGameServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<NetworkId>();
            var ecb = new EntityQueryBuilder(Allocator.Temp).WithAll<ReceiveRpcCommandRequest>().WithAll<GoInGameRequestRpc>();
            state.RequireForUpdate(state.GetEntityQuery(ecb));
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();


            foreach (var (receiveRequest, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequestRpc>().WithEntityAccess()) {
                ecb.AddComponent<NetworkStreamInGame>(receiveRequest.ValueRO.SourceConnection);

                //Debug.Log("[GoInGameServerSystem]Connected: " + entity);

                var playerEntity = ecb.Instantiate(entitiesReferences.PlayerPrefabEntity);
                var spawnPosition = new float3() { x = UnityEngine.Random.Range(-10, 10), y = 0, z = 0 };
                ecb.SetComponent(playerEntity, LocalTransform.FromPosition(spawnPosition));

                var ownerId = SystemAPI.GetComponent<NetworkId>(receiveRequest.ValueRO.SourceConnection);
                ecb.AddComponent(playerEntity, new GhostOwner() { NetworkId = ownerId.Value });

                ecb.AppendToBuffer(receiveRequest.ValueRO.SourceConnection, new LinkedEntityGroup() { Value = playerEntity});

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}
