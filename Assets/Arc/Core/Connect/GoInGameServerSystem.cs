using Arc.Core.Player;
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
            state.RequireForUpdate<NetworkId>();

            var rpcQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ReceiveRpcCommandRequest, GoInGameRequestRpc>();
            state.RequireForUpdate(state.GetEntityQuery(rpcQuery));
            rpcQuery.Dispose();

            var spawnerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerSpawnerTag, Simulate>();
            state.RequireForUpdate(state.GetEntityQuery(spawnerQuery));
            spawnerQuery.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var spawnerQuery = SystemAPI.QueryBuilder().WithAll<PlayerSpawnerTag, PlayerSpawnerData, LocalTransform>().Build();
            var spawnerDataArray = spawnerQuery.ToComponentDataArray<PlayerSpawnerData>(state.WorldUpdateAllocator);
            var spawnerTransformArray = spawnerQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

            foreach (var (requestRpc, receiveRequest, entity) in SystemAPI.Query<RefRO<GoInGameRequestRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess()) {
                ecb.AddComponent<NetworkStreamInGame>(receiveRequest.ValueRO.SourceConnection);
                var spawnIndex = requestRpc.ValueRO.SpawnerIndex;
                //Debug.Log("[GoInGameServerSystem]Connected: " + entity + " " + spawnIndex);

                var playerEntity = ecb.Instantiate(spawnerDataArray[spawnIndex].PrefabEntity);
                ecb.SetComponent(playerEntity, LocalTransform.FromPosition(spawnerTransformArray[spawnIndex].Position));

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
