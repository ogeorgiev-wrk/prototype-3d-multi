using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Player {
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class PlayerInputClientSystem : SystemBase {
        private InputSystem_Actions _inputActions;

        protected override void OnCreate() {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
        }

        protected override void OnUpdate() {
            var currentInput = _inputActions.Player.Move.ReadValue<Vector2>();

            foreach(var playerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>()) {
                playerInput.ValueRW.InputVector = (float2)currentInput;
            }
        }
    }
}
