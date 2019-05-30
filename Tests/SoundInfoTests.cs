using ElevatorMusic.Playback;
using System;
using Xunit;

namespace ElevatorMusic.Tests
{
    public class SoundInfoTests
    {
        [Fact]
        public void GetSoundInfoTest()
        {            
            Assert.Equal(193202, SoundInfo.GetSoundLengthMs("Music/Lobby Time.wav"));
            Assert.Equal(166949, SoundInfo.GetSoundLengthMs("Music/I Knew a Guy.wav"));
            Assert.Equal(189205, SoundInfo.GetSoundLengthMs("Music/Local Forecast - Elevator.wav"));
        }
    }
}
