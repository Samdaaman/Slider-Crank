//////////////////////////////////////////////////////////////////////////////
///
/// WaveStream processor class for manipulating audio stream in C# with 
/// SoundTouch library.
/// 
/// This module uses NAudio library for C# audio file input / output
/// 
/// Author        : Copyright (c) Olli Parviainen
/// Author e-mail : oparviai 'at' iki.fi
/// SoundTouch WWW: http://www.surina.net/soundtouch
///
////////////////////////////////////////////////////////////////////////////////
//
// License for this source code file: Microsoft Public License(Ms-PL)
//
////////////////////////////////////////////////////////////////////////////////

using conrod;
using NAudio.Dsp;
using NAudio.Wave;
using soundtouch;
using System;

namespace csharp_example
{
    /// <summary>
    /// Helper class that allow writing status texts to the host application
    /// </summary>
    public class StatusMessage
    {
        /// <summary>
        /// Handler for status message events. Subscribe this from the host application
        /// </summary>
        public static event EventHandler<string> statusEvent;

        /// <summary>
        /// Pass a status message to the host application
        /// </summary>
        public static void Write(string msg)
        {
            if (statusEvent != null)
            {
                statusEvent(null, msg);
            }
        }
    }

    /// <summary>
    /// NAudui WaveStream class for processing audio stream with SoundTouch effects
    /// </summary>
    public class WaveStreamProcessor : WaveStream
    {
        private WaveChannel32 inputStr;
        private readonly Equaliser equaliser;
        public SoundTouch st;

        const int BUFFER_SIZE_BYTES = 8;
        private byte[] bytebuffer = new byte[BUFFER_SIZE_BYTES];
        private float[] floatbuffer = new float[BUFFER_SIZE_BYTES / 4];
        bool endReached = false;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">WaveChannel32 stream used for processor stream input</param>
        public WaveStreamProcessor(WaveChannel32 input, Equaliser equaliser)
        {
            inputStr = input;
            this.equaliser = equaliser;
            st = new SoundTouch();
            st.Channels = (uint)input.WaveFormat.Channels;
            st.SampleRate = (uint)input.WaveFormat.SampleRate;
        }

        /// <summary>
        /// True if end of stream reached
        /// </summary>
        public bool EndReached
        {
            get { return endReached; }
        }


        public override long Length
        {
            get
            {
                return inputStr.Length;
            }
        }


        public override long Position
        {
            get
            {
                return inputStr.Position;
            }

            set
            {
                inputStr.Position = value;
            }
        }


        public override WaveFormat WaveFormat
        {
            get
            {
                return inputStr.WaveFormat;
            }
        }

        /// <summary>
        /// Overridden Read function that returns samples processed with SoundTouch. Returns data in same format as
        /// WaveChannel32 i.e. stereo float samples.
        /// </summary>
        /// <param name="buffer">Buffer where to return sample data</param>
        /// <param name="offset">Offset from beginning of the buffer</param>
        /// <param name="count">Number of bytes to return</param>
        /// <returns>Number of bytes copied to buffer</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                // Iterate until enough samples available for output:
                // - read samples from input stream
                // - put samples to SoundStretch processor
                while (st.AvailableSampleCount < count)
                {
                    int nbytes = inputStr.Read(bytebuffer, 0, bytebuffer.Length);
                    if (nbytes == 0)
                    {
                        // end of stream. flush final samples from SoundTouch buffers to output
                        if (endReached == false)
                        {
                            endReached = true;  // do only once to avoid continuous flushing
                            st.Flush();
                        }
                        break;
                    }

                    // binary copy data from "byte[]" to "float[]" buffer
                    Buffer.BlockCopy(bytebuffer, 0, floatbuffer, 0, nbytes);
                    st.PutSamples(floatbuffer, (uint)(nbytes / 8));
                }

                // ensure that buffer is large enough to receive desired amount of data out
                if (floatbuffer.Length < count / 4)
                {
                    floatbuffer = new float[count / 4];
                }
                // get processed output samples from SoundTouch
                int numsamples = (int)st.ReceiveSamples(floatbuffer, (uint)(count / 8));
                // binary copy data from "float[]" to "byte[]" buffer
                for (int i = 0; i < count / 4; i++)
                    floatbuffer[i + offset] = equaliser.TransformSample(floatbuffer[i + offset]);

                Buffer.BlockCopy(floatbuffer, 0, buffer, offset, numsamples * 8);
                return numsamples * 8;  // number of bytes
            }
            catch (Exception exp)
            {
                StatusMessage.Write("exception in WaveStreamProcessor.Read: " + exp.Message);
                return 0;
            }
        }

        /// <summary>
        /// Clear the internal processor buffers. Call this if seeking or rewinding to new position within the stream.
        /// </summary>
        public void Clear()
        {
            st.Clear();
            endReached = false;
        }
    }


    /// <summary>
    /// Class that opens & plays MP3 file and allows real-time audio processing with SoundTouch
    /// while playing
    /// </summary>
    public class SoundProcessor
    {
        WaveFileReader waveFile;
        DirectSoundOut directSoundOut;
        public WaveStreamProcessor streamProcessor;
        private Equaliser equaliser;


        public SoundProcessor(Equaliser equaliser)
        {
            this.equaliser = equaliser;
        }


        /// <summary>
        /// Start / resume playback
        /// </summary>
        /// <returns>true if successful, false if audio file not open</returns>
        public bool Play()
        {
            if (directSoundOut == null) return false;

            if (directSoundOut.PlaybackState != PlaybackState.Playing)
            {
                directSoundOut.Play();
            }
            return true;
        }


        /// <summary>
        /// Pause playback
        /// </summary>
        /// <returns>true if successful, false if audio not playing</returns>
        public bool Pause()
        {
            if (directSoundOut == null) return false;

            if (directSoundOut.PlaybackState == PlaybackState.Playing)
            {
                directSoundOut.Stop();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Stop playback
        /// </summary>
        /// <returns>true if successful, false if audio file not open</returns>
        public bool Stop()
        {
            if (directSoundOut == null) return false;

            directSoundOut.Stop();
            waveFile.Position = 0;
            streamProcessor.Clear();
            return true;
        }

        public void SetTempo(float tempo)
        {
            if (streamProcessor != null)
                streamProcessor.st.Tempo = tempo;
        }

        
        public void RelativeSeek(int secondsToMove, int scaleFactor)
        {
            if (streamProcessor != null && waveFile != null)
            {
                long bytesToMove = Math.Max(-streamProcessor.Position, secondsToMove * waveFile.WaveFormat.AverageBytesPerSecond / scaleFactor);
                Console.WriteLine(bytesToMove);
                //streamProcessor.Clear();
                streamProcessor.Position += bytesToMove;
            }
        }


        /// <summary>
        /// Event for "playback stopped" event. 'bool' argument is true if playback has reached end of stream.
        /// </summary>
        public event EventHandler<bool> PlaybackStopped;


        /// <summary>
        /// Proxy event handler for receiving playback stopped event from WaveOut
        /// </summary>
        protected void EventHandler_stopped(object sender, StoppedEventArgs args)
        {
            bool isEnd = streamProcessor.EndReached;
            if (isEnd)
            {
                Stop();
            }
            PlaybackStopped?.Invoke(sender, isEnd);
        }


        /// <summary>
        /// Open Wav file
        /// </summary>
        /// <param name="filePath">Path to file to open</param>
        /// <returns>true if successful</returns>
        public void OpenWaveFile(string filePath)
        {
            try
            {
                Stop();
                waveFile = new WaveFileReader(filePath);
                WaveChannel32 inputStream = new WaveChannel32(waveFile);
                inputStream.PadWithZeroes = false;  // don't pad, otherwise the stream never ends

                equaliser.CreateFilters(inputStream.WaveFormat);
                streamProcessor = new WaveStreamProcessor(inputStream, equaliser);

                directSoundOut = new DirectSoundOut();
                
                directSoundOut.Init(streamProcessor);  // inputStream);
                directSoundOut.PlaybackStopped += EventHandler_stopped;

                StatusMessage.Write("Opened file " + filePath);
            }
            catch (Exception exp)
            {
                // Error in opening file
                directSoundOut = null;
                StatusMessage.Write("Can't open file: " + exp.Message);
            }

        }
    }
}
