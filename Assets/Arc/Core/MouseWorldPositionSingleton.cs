using Arc.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arc {
    public class MouseWorldPositionSingleton : MonoBehaviour {
        public static MouseWorldPositionSingleton Instance;

        private PlayerInputActions _inputActions;

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("Warning multiple instances of CameraTargetSingleton detected. Destroying new instance.", Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
        }

        public Vector3 GetPosition() {
            var lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
            var mouseCameraRay = Camera.main.ScreenPointToRay(lookInput);

            var plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(mouseCameraRay, out float distance)) {
                return mouseCameraRay.GetPoint(distance);
            } else {
                return Vector3.zero;
            }
        }

    }
}
