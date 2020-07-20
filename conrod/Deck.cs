using NAudio.Wave;
using System;
using soundtouch;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csharp_example;

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
        private Equaliser equaliser = new Equaliser();
        private SoundProcessor soundOut = null;
        private float _baseBPM = 0f;
        private float _relativeBPM = 0f;
        private int seekValueBigPrevious = 0;
        private int seekValueFinePrevious = 0;
        private int treblePercentage = 50;
        private int midPercentage = 50;
        private int bassPercentage = 50;
        
        public State CurrentState { get => _state; set => SetIfDifferent(ref _state, value); }
        public string Filename { get => _filename; private set => SetIfDifferent(ref _filename, value); }
        public float RelativeBPM { get => _relativeBPM; set => SetIfDifferent(ref _relativeBPM, value); }
        public float BaseBPM { get => _baseBPM; set => SetIfDifferent(ref _baseBPM, value); }

        public void LoadFromFile(string filename)
        {
            if (soundOut == null)
                soundOut = new SoundProcessor(equaliser);
            CurrentState = State.Loading;
            soundOut.OpenWaveFile(filename);
            RelativeBPM = BaseBPM;
            // soundOut.streamProcessor.st.Volume = 0.5f;
            Filename = filename;
            CurrentState = State.Loaded;
        }
        public void Play()
        {
           soundOut.Play();
        }
        public void Pause()
        {
            soundOut.Pause();
        }
        public void Stop()
        {
            soundOut.Stop();
        }
        private void RelativeSeekWithPrevious(ref int previousSeekValue, int newSeekValue, int scaleFactor)
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
                previousSeekValue = newSeekValue;
                soundOut.RelativeSeek(seekChange, scaleFactor);
            }
        }
        public void RelativeSeekValueChange(int seekValue, bool fine)
        {
            if (CurrentState == State.Loaded)
            {
                if (fine)
                    RelativeSeekWithPrevious(ref seekValueFinePrevious, seekValue, SEEK_SCALE_FINE);
                else
                    RelativeSeekWithPrevious(ref seekValueBigPrevious, seekValue, SEEK_SCALE_NORMAL);
            }
        }
        public void VolumeAdjust(int percentage)
        {
            equaliser.Volume = percentage / 100f;
        }
        public void TrebleAdjust(int percentage)
        {
            //treblePercentage = percentage;
            //EquilizerUpdate();
            equaliser.highBand.Gain = PercentageToDB(percentage);
        }
        public void MidAdjust(int percentage)
        {
            equaliser.midBand.Gain = PercentageToDB(percentage);
            //midPercentage = percentage;
            //EquilizerUpdate();
        }
        public void BassAdjust(int percentage)
        {
            equaliser.lowBand.Gain = PercentageToDB(percentage);
            //bassPercentage = percentage;
            //EquilizerUpdate();
        }

        private float PercentageToDB(int percentage)
        {
            return (percentage / 100f - 1) * 40f;
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
                    // equalizer.SampleFilters[i].AverageGainDB = (gains[i] - 100) / 3;
                }
            }
        }
        public void TempoAdjust(int percentage)
        {
            float tempoFactor = (percentage + 50) / 100f;
            RelativeBPM = BaseBPM * tempoFactor;
            soundOut.SetTempo(tempoFactor);
        }
    }
}
