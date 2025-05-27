using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using Arc.Core.Input;
using Unity.VisualScripting;

namespace Arc.Core.Player {
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [BurstCompile]
    public partial class PlayerInputClientSystem : SystemBase {
        private PlayerInputActions _inputActions;
        private int _weaponIndex;
        private int _weaponCount;

        [BurstCompile]
        protected override void OnCreate() {
            _inputActions = new PlayerInputActions();
            _weaponCount = 3;
        }

        [BurstCompile]
        protected override void OnStartRunning() {
            _inputActions.Enable();
        }

        [BurstCompile]
        protected override void OnStopRunning() {
            _inputActions.Disable();
        }

        [BurstCompile]
        protected override void OnUpdate() {
            foreach (var input in SystemAPI.Query<RefRW<PlayerMovementInput>>().WithAll<GhostOwnerIsLocal>()) {
                var movementInput = _inputActions.Player.Move.ReadValue<Vector2>();
                input.ValueRW.Value = (float2)movementInput;
            }

            foreach (var input in SystemAPI.Query<RefRW<PlayerTargetInput>>().WithAll<GhostOwnerIsLocal>()) {
                var lookInput = MouseWorldPositionSingleton.Instance.GetPosition();
                input.ValueRW.Value = (float3)lookInput;
            }

            foreach (var input in SystemAPI.Query<RefRW<PlayerAttackInput>>().WithAll<GhostOwnerIsLocal>()) {
                var isPrimaryAttack = _inputActions.Player.Attack.WasPressedThisFrame();
                if (isPrimaryAttack) {
                    input.ValueRW.Value.Set();
                } else {
                    input.ValueRW.Value = default;
                }
            }

            foreach (var input in SystemAPI.Query<RefRW<PlayerWeaponSwapInput>>().WithAll<GhostOwnerIsLocal>()) {
                var swapInput = _inputActions.Player.WeaponSwap.ReadValue<Vector2>();
                if (swapInput == Vector2.zero) continue;

                var increment = (int)swapInput.y;
                _weaponIndex += increment;
                if (_weaponIndex < 0) _weaponIndex = _weaponCount - 1;
                if (_weaponIndex == _weaponCount) _weaponIndex = 0;
                input.ValueRW.Value = _weaponIndex;
            }
        }

        [BurstCompile]
        protected override void OnDestroy() {

        }
    }
}
