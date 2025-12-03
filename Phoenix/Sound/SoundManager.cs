using IrrKlang;
using System.Numerics;
namespace Phoenix.Sound
{
    public class SoundManager : IDisposable
    {
        ISoundEngine _engine;
        private List<Sound> _sounds = new List<Sound>();
        
        private Vector3D _pos = new Vector3D(0, 0, 0);
        private Vector3D _dir = new Vector3D(1, 0, 0);
       
        public SoundManager()
        {
            _engine = new ISoundEngine();

            Set3DMinMaxDistance(0.5f, 200f);
            SetRollOff(.5f);
            SetDopplerFactors(1.0f, 1.0f);
        }
        public void Update()
        {
            _sounds.ForEach(s => s._instances.RemoveAll(i => i.IsFinished));
        }

        public Sound Add(string path, bool allowMultiple = true)
        {
            var src = _engine.GetSoundSource(path);
            var sound = new Sound(src, allowMultiple);
            _sounds.Add(sound);
            return sound;
        }

        public Sound3D Add3D(string path, bool allowMultiple = true)
        {
            var src = _engine.GetSoundSource(path);
            var sound = new Sound3D(src, allowMultiple);
            _sounds.Add(sound);
            return sound;
        }

        public void Toggle(SoundInstance instance)
        {
            instance.Paused = !instance.Paused;
        }
        public SoundInstance Play(Sound sound, bool loop = false, bool startPaused = false)
        {
            if (!_sounds.Contains(sound))
                return null;

            return sound.Play(ref _engine, loop, startPaused);
        }
        public SoundInstance3D Play(Sound3D sound, bool loop = false, bool startPaused = false)
        {
            if (!_sounds.Contains(sound))
                return null;

            return sound.Play(ref _engine, loop, startPaused);
        }
        public void Stop(SoundInstance sound)
        {
            sound.Stop();
        }

        public void SetListenerPositionDirection(Vector3 position, Vector3 direction, bool flipDirection = true)
        {
            _pos.Set(position.X, position.Y, position.Z);
            
            if(flipDirection)
                _dir.Set(-direction.X, direction.Y, -direction.Z);
            else
                _dir.Set(direction.X, direction.Y, direction.Z);

            _engine.SetListenerPosition(_pos, _dir);
        }
        public void Set3DMinMaxDistance(float min, float max)
        {
            _engine.Default3DSoundMinDistance = min;
            _engine.Default3DSoundMaxDistance = max;
            
        }

        public void SetRollOff(float rollOffFactor)
        {
            _engine.SetRolloffFactor(rollOffFactor);
        }
        public void SetDopplerFactors(float doppler, float distance)
        {
            _engine.SetDopplerEffectParameters(doppler, distance);
        }
        public void StopAll()
        {
            _engine.StopAllSounds();
            
        }

        public void Dispose()
        {
            _engine.Dispose();
        }
    }
}
