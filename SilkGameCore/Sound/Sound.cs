using IrrKlang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkGameCore.Sound
{
    public class Sound
    {
        public ISoundSource Source { get;}

        public bool AllowMultiple { get; set; }
        internal List<SoundInstance> _instances = new List<SoundInstance>();

        public Sound(ISoundSource src, bool allowMultiple)
        {
            Source = src;
            AllowMultiple = allowMultiple;
        }
        
        public virtual SoundInstance Play(ref ISoundEngine engine, bool loop, bool startPaused)
        {
            if(!AllowMultiple)
            {
                if (_instances.Count > 0)
                    _instances[0].Stop();
                _instances.Clear();
            }

            var isound = engine.Play2D(Source, loop, startPaused, false);
            var instance = new SoundInstance(isound, loop, startPaused);
            _instances.Add(instance);

            return instance;
        }

    }
}
