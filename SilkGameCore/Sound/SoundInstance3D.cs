using IrrKlang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SilkGameCore.Sound
{
    public class SoundInstance3D : SoundInstance
    {
        
        public SoundInstance3D(ISound iSound, bool loop, bool startPaused) : base(iSound, loop, startPaused)
        {
        }

        Vector3D _pos = new Vector3D(0, 0, 0);
        Vector3D _vel = new Vector3D(0, 0, 0);

        public void SetPosition(Vector3 pos)
        {
            _pos.Set(pos.X, pos.Y, pos.Z);
            ISound.Position = _pos;
            
        }

        public void SetVelocity(Vector3 vel)
        {
            _vel.Set(vel.X, vel.Y, vel.Z);
            ISound.Velocity = _vel;
        }


    }
}
