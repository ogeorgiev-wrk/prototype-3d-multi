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
            foreach (var input in SystemAPI.Query<RefRW<PlayerMovementInput>>().WithAll<GhostOwnerIsLocal>()) {
                var movementInput = _inputActions.Player.Move.ReadValue<Vector2>();
                input.ValueRW.Value = (float2)movementInput;
            }

            foreach (var input in SystemAPI.Query<RefRW<PlayerLookInput>>().WithAll<GhostOwnerIsLocal>()) {
                var lookInput = MouseWorldPositionSingleton.Instance.GetPosition();
                input.ValueRW.Value = (float3)lookInput;
            }


            foreach (var input in SystemAPI.Query<RefRW<PlayerAttackInput>>().WithAll<GhostOwnerIsLocal>()) {
                var isPrimaryAttack = _inputActions.Player.PrimaryAttack.WasPressedThisFrame();
                if (isPrimaryAttack) {
                    input.ValueRW.PrimaryAttack.Set();
                } else {
                    input.ValueRW.PrimaryAttack = default;
                }

                var isSecondaryAttack = _inputActions.Player.SecondaryAttack.IsPressed();
                if (isSecondaryAttack) {
                    input.ValueRW.SecondaryAttack.Set();
                } else {
                    input.ValueRW.SecondaryAttack = default;
                }
            }
        }
    }
}
