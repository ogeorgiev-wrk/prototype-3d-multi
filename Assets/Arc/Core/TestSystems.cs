using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using Unity.Mathematics;
using Arc.Core.Player;
using Unity.Transforms;

namespace Arc.Core {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    partial struct TestServerSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (rpc, request, entity) in SystemAPI.Query<RefRO<SimpleRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess()) {
                Debug.Log("RPC: " + rpc.ValueRO.Value + " :: " + entity + " :: " + request.ValueRO.SourceConnection);
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }


    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct TestClientSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            /*
            foreach (var playerInput in SystemAPI.Query<RefRO<PlayerInput>>()) {
                var isAttacking = playerInput.ValueRO.AttackInput.IsSet;
                if (!isAttacking) continue;

                var rpcEntity = ecb.CreateEntity();
                ecb.AddComponent(rpcEntity, new SimpleRpc() { Value = isAttacking });
                ecb.AddComponent(rpcEntity, new SendRpcCommandRequest() { });
            }
            */

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }

    public struct SimpleRpc : IRpcCommand {
        public bool Value;
    }
}
