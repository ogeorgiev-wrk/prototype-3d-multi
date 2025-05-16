using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class PlayerInputClientSystem : SystemBase {
        private InputSystem_Actions _inputActions;

        protected override void OnCreate() {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
        }

        protected override void OnUpdate() {
            var movementInput = _inputActions.Player.Move.ReadValue<Vector2>();
            var isShooting = _inputActions.Player.Attack.IsPressed();

            foreach(var playerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>()) {
                playerInput.ValueRW.MovementInput = (float2)movementInput;
                if (isShooting) {
                    playerInput.ValueRW.AttackInput.Set();
                } else {
                    playerInput.ValueRW.AttackInput = default;
                }
            }
        }
    }
}
