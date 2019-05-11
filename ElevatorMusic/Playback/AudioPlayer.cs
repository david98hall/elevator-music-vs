using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorMusic.Playback
{
    // Source: https://stackoverflow.com/a/29906524/5174527
    internal class AudioPlayer : SoundPlayer
    {
        internal bool Finished { get; private set; }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private CancellationToken cancellationToken;
        private readonly string fileName;
        private bool playingAsync = false;

        internal event EventHandler SoundFinished;

        public AudioPlayer(string soundLocation) : base(soundLocation)
        {
            fileName = soundLocation;
            cancellationToken = tokenSource.Token;
        }

        internal async Task PlayAsync()
        {
            Finished = false;
            playingAsync = true;
            await Task.Run(() =>
            {
                try
                {
                    double lenMs = SoundInfo.GetSoundLength(fileName);
                    DateTime stopAt = DateTime.Now.AddMilliseconds(lenMs);
                    this.Play();
                    while (DateTime.Now < stopAt)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        //The delay helps reduce processor usage while "spinning"
                        Task.Delay(10).Wait();
                    }
                }
                catch (OperationCanceledException)
                {
                    base.Stop();
                }
                finally
                {
                    OnSoundFinished();
                }

            }, cancellationToken);
        }

        internal new void Stop()
        {
            if (playingAsync)
                tokenSource.Cancel();
            else
                base.Stop();   //To stop the SoundPlayer Wave file
        }

        protected virtual void OnSoundFinished()
        {
            Finished = true;
            playingAsync = false;

            SoundFinished?.Invoke(this, EventArgs.Empty);
        }
    }

}
