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
            Assert.Equal(193202, SoundInfo.GetWavLengthMs("Music/Lobby Time.wav"));
            Assert.Equal(166949, SoundInfo.GetWavLengthMs("Music/I Knew a Guy.wav"));
            Assert.Equal(189206, SoundInfo.GetWavLengthMs("Music/Local Forecast - Elevator.wav"));

            Assert.Equal(193, SoundInfo.GetWavLength("Music/Lobby Time.wav"));
            Assert.Equal(167, SoundInfo.GetWavLength("Music/I Knew a Guy.wav"));
            Assert.Equal(189, SoundInfo.GetWavLength("Music/Local Forecast - Elevator.wav"));
        }
    }
}
