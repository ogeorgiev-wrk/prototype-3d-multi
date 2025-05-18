using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Arc.Core.Input;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class PlayerInputClientSystem : SystemBase {
        private PlayerInputActions _inputActions;

        protected override void OnCreate() {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
        }

        protected override void OnUpdate() {
            var movementInput = _inputActions.Player.Move.ReadValue<Vector2>();
            var lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
            var isShooting = _inputActions.Player.Attack.IsPressed();

            foreach(var playerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>()) {
                playerInput.ValueRW.MovementInput = (float2)movementInput;

                var lookInputViewport = Camera.main.ScreenToViewportPoint(new Vector3(lookInput.x, lookInput.y, 0f));
                playerInput.ValueRW.LookInput = new float2(lookInputViewport.x - 0.5f, lookInputViewport.y - 0.5f);

                if (isShooting) {
                    playerInput.ValueRW.AttackInput.Set();
                } else {
                    playerInput.ValueRW.AttackInput = default;
                }
            }
        }
    }
}
