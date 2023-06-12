using System;
using UnityEngine;
using Varneon.VUdon.Udonity.Fields;

namespace Varneon.VUdon.Udonity.Editors
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    public class AudioSourceEditor : Abstract.ComponentEditor
    {
        [SerializeField]
        private ObjectField audioClipField;

        [SerializeField]
        private Toggle muteField;

        [SerializeField]
        private Toggle spatializeField;

        [SerializeField]
        private Toggle spatializePostEffectsField;

        [SerializeField]
        private Toggle bypassReverbZonesField;

        [SerializeField]
        private Toggle playOnAwakeField;

        [SerializeField]
        private Toggle loopField;

        [SerializeField]
        private FloatField priorityField;

        [SerializeField]
        private FloatField volumeField;

        [SerializeField]
        private FloatField pitchField;

        [SerializeField]
        private FloatField stereoPanField;

        [SerializeField]
        private FloatField spatialBlendField;

        [SerializeField]
        private FloatField reverbZoneMixField;

        [SerializeField]
        private FloatField dopplerLevelField;

        [SerializeField]
        private FloatField spreadField;

        [SerializeField]
        private EnumField volumeRolloffField;

        [SerializeField]
        private FloatField minDistanceField;

        [SerializeField]
        private FloatField maxDistanceField;

        public override Type InspectedType => typeof(AudioSource);

        private AudioSource target;

        protected override void OnEditorInitialized(Component component)
        {
            target = (AudioSource)component;

            audioClipField.SetFieldType(typeof(AudioClip));

            UpdateFields();
        }

        public void UpdateFields()
        {
            audioClipField.SetValueWithoutNotify(target.clip);

            muteField.SetValueWithoutNotify(target.mute);

            spatializeField.SetValueWithoutNotify(target.spatialize);

            spatializePostEffectsField.SetValueWithoutNotify(target.spatializePostEffects);

            bypassReverbZonesField.SetValueWithoutNotify(target.bypassReverbZones);

            playOnAwakeField.SetValueWithoutNotify(target.playOnAwake);

            loopField.SetValueWithoutNotify(target.loop);

            priorityField.SetValueWithoutNotify(target.priority);

            volumeField.SetValueWithoutNotify(target.volume);

            pitchField.SetValueWithoutNotify(target.pitch);

            stereoPanField.SetValueWithoutNotify(target.panStereo);

            spatialBlendField.SetValueWithoutNotify(target.spatialBlend);

            reverbZoneMixField.SetValueWithoutNotify(target.reverbZoneMix);

            dopplerLevelField.SetValueWithoutNotify(target.dopplerLevel);

            spreadField.SetValueWithoutNotify(target.spread);

            volumeRolloffField.SetValueWithoutNotify((int)target.rolloffMode);

            minDistanceField.SetValueWithoutNotify(target.minDistance);

            maxDistanceField.SetValueWithoutNotify(target.maxDistance);

            if (Expanded) { SendCustomEventDelayedFrames(nameof(UpdateFields), 0); }
        }

        public void OnAudioClipChanged()
        {
            target.clip = (AudioClip)audioClipField.Value;
        }

        public void OnMuteChanged()
        {
            target.mute = muteField.Value;
        }

        public void OnSpatializeChanged()
        {
            target.spatialize = spatializeField.Value;
        }

        public void OnSpatializePostEffectsChanged()
        {
            target.spatializePostEffects = spatializePostEffectsField.Value;
        }

        public void OnBypassReverbZonesChanged()
        {
            target.bypassReverbZones = bypassReverbZonesField.Value;
        }

        public void OnPlayOnAwakeChanged()
        {
            target.playOnAwake = playOnAwakeField.Value;
        }

        public void OnLoopChanged()
        {
            target.loop = loopField.Value;
        }

        public void OnPriorityChanged()
        {
            target.priority = (int)priorityField.Value;
        }

        public void OnVolumeChanged()
        {
            target.volume = volumeField.Value;
        }

        public void OnPitchChanged()
        {
            target.pitch = pitchField.Value;
        }

        public void OnStereoPanChanged()
        {
            target.panStereo = stereoPanField.Value;
        }

        public void OnSpatialBlendChanged()
        {
            target.spatialBlend = spatialBlendField.Value;
        }

        public void OnReverbZoneMixChanged()
        {
            target.reverbZoneMix = reverbZoneMixField.Value;
        }

        public void OnDopplerLevelChanged()
        {
            target.dopplerLevel = dopplerLevelField.Value;
        }

        public void OnSpreadChanged()
        {
            target.spread = spreadField.Value;
        }

        public void OnVolumeRolloffChanged()
        {
            target.rolloffMode = (AudioRolloffMode)volumeRolloffField.Value;
        }

        public void OnMinDistanceChanged()
        {
            target.minDistance = minDistanceField.Value;
        }

        public void OnMaxDistanceChanged()
        {
            target.maxDistance = maxDistanceField.Value;
        }

        protected override void OnToggleExpanded(bool expanded)
        {
            if (expanded) { UpdateFields(); }
        }

        protected override void OnInitializedOnBuild()
        {
            audioClipField.RegisterValueChangedCallback(this, nameof(OnAudioClipChanged));

            muteField.RegisterValueChangedCallback(this, nameof(OnMuteChanged));

            spatializeField.RegisterValueChangedCallback(this, nameof(OnSpatializeChanged));

            spatializePostEffectsField.RegisterValueChangedCallback(this, nameof(OnSpatializePostEffectsChanged));

            bypassReverbZonesField.RegisterValueChangedCallback(this, nameof(OnBypassReverbZonesChanged));

            playOnAwakeField.RegisterValueChangedCallback(this, nameof(OnPlayOnAwakeChanged));

            loopField.RegisterValueChangedCallback(this, nameof(OnLoopChanged));

            priorityField.RegisterValueChangedCallback(this, nameof(OnPriorityChanged));

            volumeField.RegisterValueChangedCallback(this, nameof(OnVolumeChanged));

            pitchField.RegisterValueChangedCallback(this, nameof(OnPitchChanged));

            stereoPanField.RegisterValueChangedCallback(this, nameof(OnStereoPanChanged));

            spatialBlendField.RegisterValueChangedCallback(this, nameof(OnSpatialBlendChanged));

            reverbZoneMixField.RegisterValueChangedCallback(this, nameof(OnReverbZoneMixChanged));

            dopplerLevelField.RegisterValueChangedCallback(this, nameof(OnDopplerLevelChanged));

            spreadField.RegisterValueChangedCallback(this, nameof(OnSpreadChanged));

            volumeRolloffField.RegisterValueChangedCallback(this, nameof(OnVolumeRolloffChanged));

            minDistanceField.RegisterValueChangedCallback(this, nameof(OnMinDistanceChanged));

            maxDistanceField.RegisterValueChangedCallback(this, nameof(OnMaxDistanceChanged));
        }
    }
}
