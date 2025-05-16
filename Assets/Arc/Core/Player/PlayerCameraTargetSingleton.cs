using UnityEngine;

namespace Arc.Core.Player {

    public class PlayerCameraTargetSingleton : MonoBehaviour {
        public static PlayerCameraTargetSingleton Instance;

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("Warning multiple instances of CameraTargetSingleton detected. Destroying new instance.", Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }

}
