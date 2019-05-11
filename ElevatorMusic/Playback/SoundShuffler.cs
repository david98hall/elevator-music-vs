using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorMusic.Playback
{
    internal class SoundShuffler
    {
        #region Sound related fields
        private readonly IDictionary<string, SoundPlayer> soundPlayers = new Dictionary<string, SoundPlayer>();
        private readonly Stack<string> previousSounds = new Stack<string>();
        private readonly Stack<string> nextSounds = new Stack<string>();
        private volatile string currentSound;
        private volatile bool pause;
        #endregion

        #region Threading fields
        private CancellationTokenSource tokenSource;
        #endregion

        #region Constructors
        internal SoundShuffler()
        {
            ResetCancellationTokenSource();
        }
        #endregion

        #region Collection management
        internal void AddSound(string filepath)
        {
            if (File.Exists(filepath))
            {
                soundPlayers.Add(filepath, new SoundPlayer(@filepath));
            }
        }

        internal void RemoveSound(string filepath)
        {
            soundPlayers.Remove(filepath);
        }

        internal void Clear()
        {
            soundPlayers.Clear();
            Reset();
        }

        internal void Reset()
        {
            currentSound = null;
            previousSounds.Clear();
            nextSounds.Clear();
        }
        #endregion

        #region Playback
        internal async Task ShufflePlayAsync(bool looping)
        {
            // Stop and reset the shuffler
            StopPlayback();
            Reset();

            // Shuffle the sounds
            var players = soundPlayers.Keys.ToList();
            var random = new Random();
            while (nextSounds.Count < soundPlayers.Count)
            {
                nextSounds.Push(players[random.Next(players.Count)]);
            }

            // Play the sounds in the shuffled order
            await StartPlaybackAsync(looping);
        }

        internal async Task StartPlaybackAsync(bool looping)
        {
            await StartPlaybackAsync(looping, tokenSource.Token);
        }

        private async Task StartPlaybackAsync(bool looping, CancellationToken token)
        {
            await Task.Run(async () =>
            {
                // Play all sounds (break if there's a cancellation request)
                while (nextSounds.Count > 0 && !token.IsCancellationRequested)
                {
                    // Get the next sound to play
                    currentSound = nextSounds.Pop();

                    // Play the current sound if the player is available
                    if (soundPlayers.TryGetValue(currentSound, out SoundPlayer player))
                    {
                        token.Register(() => player.Stop());
                        player.PlaySync();
                    }

                    // Store the played sound in the history
                    previousSounds.Push(currentSound);
                }

                // Reset the current sound
                currentSound = null;

                if (looping && (!token.IsCancellationRequested || !pause))
                {
                    // Use the sound history to reset the shuffled order to its initial state
                    while (previousSounds.Count() > 0)
                    {
                        nextSounds.Push(previousSounds.Pop());
                    }

                    // Start the playback again
                    if (!token.IsCancellationRequested)
                    {
                        await StartPlaybackAsync(looping, token);
                    }
                }

            }, token);
        }

        private void StopPlayback()
        {
            StopPlayback(false);
        }

        private void StopPlayback(bool pause)
        {
            this.pause = pause;

            // Cancel any running thread
            tokenSource.Cancel();

            // Stop any active playback
            /*
            foreach (var player in soundPlayers.Values)
            {
                player.Stop();
            }
            */
        }
        #endregion

        #region Threading
        private void ResetCancellationTokenSource()
        {
            tokenSource = new CancellationTokenSource();
            tokenSource.Token.Register(() => ResetCancellationTokenSource());
        }
        #endregion

    }
}
