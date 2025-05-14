using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.NetCode;
using UnityEngine;

/*
namespace Arc.Core {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    partial class TestClientSystem : SystemBase {
        private InputSystem_Actions _inputActions;

        protected override void OnCreate() {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
        }
        protected override void OnUpdate() {
            var currentInput = _inputActions.Player.Move.ReadValue<Vector2>();
            if (currentInput == Vector2.zero) return;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new SimpleRpc() { value = currentInput });
            ecb.AddComponent(rpcEntity, new SendRpcCommandRequest() { });

            ecb.Playback(EntityManager);
            Debug.Log(currentInput);
        }
    }

    public struct SimpleRpc : IRpcCommand {
        public Vector2 value;
    }
}
*/
