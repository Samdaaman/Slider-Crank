using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conrod
{
    public class Deck: SetIfDifferentHelper
    {
        public enum State
        {
            Empty,
            Loading,
            Loaded
        }
        private string _filename = null;
        private State _state = State.Empty;

        private DirectSoundOut soundOut = null;
        
        public State CurrentState { get => _state; set => SetIfDifferent(ref _state, value); }
        public string Filename { get => _filename; private set => SetIfDifferent(ref _filename, value); }


        public void LoadFromFile(string filename)
        {
            CurrentState = State.Loading;
            IWaveSource waveSource = CodecFactory.Instance.GetCodec(filename);
            soundOut?.Dispose();
            soundOut = new DirectSoundOut();
            soundOut.Initialize(waveSource);
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
    }
}
