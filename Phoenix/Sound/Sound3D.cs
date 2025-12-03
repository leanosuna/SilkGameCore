using IrrKlang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Sound
{
    public class Sound3D : Sound
    {

        public Sound3D(ISoundSource src, bool allowMultiple) : base(src, allowMultiple)
        {

        }

        public override SoundInstance3D Play(ref ISoundEngine engine, bool loop, bool startPaused)
        {
            if (!AllowMultiple)
            {
                if (_instances.Count > 0)
                    _instances[0].Stop();
                _instances.Clear();
            }

            var isound = engine.Play3D(Source, 0,0,0,loop,startPaused, false);
            var instance = new SoundInstance3D(isound, loop, startPaused);
            _instances.Add(instance);

            return instance;
        }
    }
}
