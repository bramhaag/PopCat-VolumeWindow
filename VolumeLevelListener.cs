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
            var playbackDevice = new CoreAudioController().DefaultPlaybackDevice;
            playbackDevice.MuteChanged.Subscribe(e =>
                OnVolumeLevelChange?.Invoke(this,
                    new VolumeLevelEventArgs {Level = e.IsMuted ? 0 : (int) e.Device.Volume})
            );

            playbackDevice.VolumeChanged.Subscribe(e => 
                OnVolumeLevelChange?.Invoke(this, new VolumeLevelEventArgs {Level = (int) e.Volume})
            );
        }

        public class VolumeLevelEventArgs : EventArgs
        {
            public int Level { get; init; }
        }
    }
}