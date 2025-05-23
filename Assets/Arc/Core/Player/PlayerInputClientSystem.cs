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
    [BurstCompile]
    public partial class PlayerInputClientSystem : SystemBase {
        private PlayerInputActions _inputActions;

        [BurstCompile]
        protected override void OnCreate() {
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
        }

        [BurstCompile]
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
                var isPrimaryAttack = _inputActions.Player.Attack.IsPressed();
                if (isPrimaryAttack) {
                    input.ValueRW.Value.Set();
                } else {
                    input.ValueRW.Value = default;
                }
            }
        }
    }
}
