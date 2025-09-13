using ManagedBass;
namespace SilkGameCore.Sound
{
    public class SoundManager
    {
        public SoundManager()
        {

        }

        static int currentStream;
        public void Play(string path)
        {
            if (!Bass.Init())
            {
                Console.WriteLine("Sound manager init fail");

                return;
            }
            currentStream = Bass.CreateStream(path, 0, 0, BassFlags.Default);
            if (currentStream == 0)
            {
                Console.WriteLine("Failed to load audio file.");

                Console.WriteLine($"last error {Bass.LastError}"); // Shows why it failed
                return;
            }
            Bass.ChannelPlay(currentStream);

        }

        public void Play3D(string path)
        {
            if (!Bass.Init())
            {
                Console.WriteLine("Sound manager init fail");

                return;
            }
            currentStream = Bass.CreateStream(path, 0, 0, BassFlags.Default);
            if (currentStream == 0)
            {
                Console.WriteLine("Failed to load audio file.");

                Console.WriteLine($"last error {Bass.LastError}"); // Shows why it failed
                return;
            }
            Bass.ChannelPlay(currentStream);

        }

        public void Stop()
        {
            Bass.StreamFree(currentStream);
            Bass.Free();
        }
    }
}
