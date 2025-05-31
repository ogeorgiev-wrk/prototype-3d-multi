using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Arc.Core.Input;

namespace Arc.Core.UI {
    public class GameUISingleton : MonoBehaviour {
        public static GameUISingleton Instance;

        [SerializeField] private List<Sprite> WeaponSpriteList;

        [SerializeField] private Image _weaponImage;

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("Warning multiple instances of CameraTargetSingleton detected. Destroying new instance.", Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            _weaponImage.sprite = WeaponSpriteList[0];
        }

        public void SelectWeapon(int index) => SelectWeaponInternal(index);

        private void SelectWeaponInternal(int index) {
            _weaponImage.sprite = WeaponSpriteList[index];
        }
    }
}
