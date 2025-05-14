using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace Arc.Core {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct TestServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach(var (rpc, request, entity) in SystemAPI.Query<RefRO<SimpleRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess()) {
                //Debug.Log("RPC: " + rpc.ValueRO.value + " :: " + entity + " :: " + request.ValueRO.SourceConnection);
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    public struct SimpleRpc : IRpcCommand {
        public Vector2 value;
    }
}
