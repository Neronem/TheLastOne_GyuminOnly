using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace _1.Scripts.VisualEffects
{
    public class PostProcessEditForFocus : MonoBehaviour
    {
        [Header("Volume Component")]
        [SerializeField] private Volume volume;
        [SerializeField] private ColorAdjustments colorAdjustments;
        [SerializeField] private Bloom bloom;
        
        [Header("focus Settings")]
        [SerializeField] private float defaultExposure = 0.6f;
        [SerializeField] private float defaultBloomIntensity = 1f;
        [SerializeField] private float focusExposure = -1.3f;
        [SerializeField] private float focusBloomIntensity = 7f;
        [SerializeField] private float transitionTime = 1f;
        
        private void Awake()
        {
            if (volume.profile.TryGet(out colorAdjustments)) { colorAdjustments.postExposure.overrideState = true; }
            if (volume.profile.TryGet(out bloom)) { bloom.intensity.overrideState = true; }
            FocusModeOnOrNot(false);
        }

        /// <summary>
        /// Focus모드 진입 또는 퇴장
        /// </summary>
        /// <param name="isOn"></param>
        public void FocusModeOnOrNot(bool isOn)
        {
            float exposureValue = isOn ? focusExposure : defaultExposure;
            float bloomValue = isOn ? focusBloomIntensity : defaultBloomIntensity;

            DOTween.To(
                () => colorAdjustments.postExposure.value,
                x => colorAdjustments.postExposure.value = x,
                exposureValue,
                transitionTime
            );

            DOTween.To(
                () => bloom.intensity.value,
                x => bloom.intensity.value = x,
                bloomValue,
                transitionTime
            );
        }
    }
}
