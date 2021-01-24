using System;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

namespace PopCat
{
    public class VolumeLevelListener
    {
        public EventHandler<VolumeLevelEventArgs> OnVolumeLevelChange;

        public void Start()
        {
            new CoreAudioController().DefaultPlaybackDevice.VolumeChanged.Subscribe(e =>
                OnVolumeLevelChange?.Invoke(this, new VolumeLevelEventArgs {Level = (int) e.Volume})
            );
        }

        public class VolumeLevelEventArgs : EventArgs
        {
            public int Level { get; init; }
        }
    }
}