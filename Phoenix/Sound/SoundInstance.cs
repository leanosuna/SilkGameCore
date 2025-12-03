using IrrKlang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Sound
{
    public class SoundInstance
    {
        internal ISound ISound { get; }

        public bool Loop { get; }
        public bool StartPaused { get; }

        public float Volume
        {
            get => ISound.Volume;
            set => ISound.Volume = value;
        }
        public float Speed
        {
            get => ISound.PlaybackSpeed;
            set => ISound.PlaybackSpeed = value;
        }
        public bool IsFinished => ISound.Finished;
        
        public bool Paused
        {
            get => ISound.Paused;
            set => ISound.Paused = value;
        }

        public SoundInstance(ISound iSound, bool loop, bool startPaused)
        {
            ISound = iSound;
            Loop = loop;
            StartPaused = startPaused;
        }
        internal void Stop()
        {
            ISound.Stop();
        }


    }
}
