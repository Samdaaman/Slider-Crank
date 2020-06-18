using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams.Effects;
using CSCore.Streams;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conrod
{
    public class Deck: SetIfDifferentHelper
    {
        const int SEEK_SCALE_NORMAL = 100;
        const int SEEK_SCALE_FINE = 1000;

        public enum State
        {
            Empty,
            Loading,
            Loaded
        }
        private string _filename = null;
        private State _state = State.Empty;

        private DirectSoundOut soundOut = null;
        private Equalizer equalizer = null;
        private int seekValueBigPrevious = 0;
        private int seekValueFinePrevious = 0;
        private int treblePercentage = 50;
        private int midPercentage = 50;
        private int bassPercentage = 50;
        
        public State CurrentState { get => _state; set => SetIfDifferent(ref _state, value); }
        public string Filename { get => _filename; private set => SetIfDifferent(ref _filename, value); }


        public void LoadFromFile(string filename)
        {
            CurrentState = State.Loading;
            IWaveSource waveSource = CodecFactory.Instance.GetCodec(filename);
            equalizer?.Dispose();
            soundOut?.Dispose();
            soundOut = new DirectSoundOut(latency: 50);
            equalizer = Equalizer.Create10BandEqualizer(waveSource.ToSampleSource());
            soundOut.Initialize(equalizer.ToWaveSource());
            soundOut.Volume = 0.5f;
            Filename = filename;
            CurrentState = State.Loaded;
        }
        public void Play()
        {
            if (CurrentState == State.Loaded && (soundOut.PlaybackState == PlaybackState.Paused || soundOut.PlaybackState == PlaybackState.Stopped))
                soundOut.Play();
        }
        public void Pause()
        {
            if (CurrentState == State.Loaded && (soundOut.PlaybackState == PlaybackState.Playing))
                soundOut.Pause();
        }
        public void Stop()
        {
            if (CurrentState == State.Loaded && soundOut != null)
            {
                soundOut.Dispose();
                soundOut = null;
                Filename = null;
                CurrentState = State.Empty;
            }
        }
        private void relativeSeekWithPrevious(ref int previousSeekValue, int newSeekValue, int scaleFactor)
        {
            if (newSeekValue == -1)
            {
                previousSeekValue = 0;
            }
            else
            {
                while (newSeekValue - previousSeekValue > 500)
                    newSeekValue -= 1000;
                while (previousSeekValue - newSeekValue < -500)
                    newSeekValue += 1000;
                int seekChange = newSeekValue - previousSeekValue;
                long scaledChange = Convert.ToInt64(Math.Round((decimal)seekChange * soundOut.WaveSource.WaveFormat.BytesPerSecond * 2 / scaleFactor) / 2);
                if (scaledChange != 0)
                {
                    Console.WriteLine(scaledChange);
                    previousSeekValue = newSeekValue;
                    soundOut.WaveSource.Position += Math.Max(-soundOut.WaveSource.Position, scaledChange);
                }
                else
                    Console.WriteLine("Current seek change would be zero");
            }
        }
        public void RelativeSeekValueChange(int seekValue, bool fine)
        {
            if (soundOut?.PlaybackState == PlaybackState.Paused || soundOut?.PlaybackState == PlaybackState.Playing)
            {
                if (fine)
                    relativeSeekWithPrevious(ref seekValueFinePrevious, seekValue, SEEK_SCALE_FINE);
                else
                    relativeSeekWithPrevious(ref seekValueBigPrevious, seekValue, SEEK_SCALE_NORMAL);
            }
        }
        public void VolumeAdjust(int percentage)
        {
            soundOut.Volume = percentage / 100f;
        }
        public void TrebleAdjust(int percentage)
        {
            treblePercentage = percentage;
            EquilizerUpdate();
        }
        public void MidAdjust(int percentage)
        {
            midPercentage = percentage;
            EquilizerUpdate();
        }
        public void BassAdjust(int percentage)
        {
            bassPercentage = percentage;
            EquilizerUpdate();
        }
        private void EquilizerUpdate()
        {
            if (CurrentState == State.Loaded)
            {
                double[] gains = new double[10];
                gains[0] = bassPercentage - 50;
                gains[1] = bassPercentage * 0.8 + midPercentage * 0.2 - 50;
                gains[2] = bassPercentage * 0.4 + midPercentage * 0.6 - 50;
                gains[3] = bassPercentage * 0.1 + midPercentage * 0.9 - 50;
                gains[4] = midPercentage - 50;
                gains[5] = treblePercentage * 0.1 + midPercentage * 0.9 - 50;
                gains[6] = treblePercentage * 0.4 + midPercentage * 0.6 - 50;
                gains[7] = treblePercentage * 0.8 + midPercentage * 0.2 - 50;
                gains[8] = treblePercentage;

                //gains[0] = gains[1] = gains[2] = (float)bassPercentage;
                //gains[3] = gains[4] = gains[5] = gains[6] = (float)midPercentage;
                //gains[7] = gains[8] = gains[9] = (float)treblePercentage;

                for (int i = 0; i < gains.Length; i++)
                {
                    equalizer.SampleFilters[i].AverageGainDB = (gains[i] - 100) / 3;
                }
            }
        }
    }
}
