using NAudio.Dsp;
using NAudio.Wave;

namespace conrod
{
    public class Equaliser
    {
        public readonly Band lowBand;
        public readonly Band midBand;
        public readonly Band highBand;
        private BiQuadFilter lowFilter;
        private BiQuadFilter midFilter;
        private BiQuadFilter highFilter;
        private WaveFormat inputFormat;

        public float Volume = 1.0f;

        public Equaliser()
        {
            lowBand = new Band(this, 10, 0.5f) { Gain = 1f };
            midBand = new Band(this, 300, 0.5f) { Gain = 1f };
            highBand = new Band(this, 800, 0.5f) { Gain = 1f };
        }
        public float TransformSample(float inSample)
        {
            return highFilter.Transform(midFilter.Transform(lowFilter.Transform(inSample))) * Volume;
        }

        public void CreateFilters(WaveFormat inputFormat)
        {
            this.inputFormat = inputFormat;
            lowFilter = BiQuadFilter.PeakingEQ(inputFormat.SampleRate, lowBand.frequency, lowBand.bandwidth, lowBand.Gain);
            midFilter = BiQuadFilter.PeakingEQ(inputFormat.SampleRate, midBand.frequency, midBand.bandwidth, midBand.Gain);
            highFilter = BiQuadFilter.PeakingEQ(inputFormat.SampleRate, highBand.frequency, highBand.bandwidth, highBand.Gain);
        }

        public void UpdateFilters()
        {
            lowFilter?.SetPeakingEq(inputFormat.SampleRate, lowBand.frequency, lowBand.bandwidth, lowBand.Gain);
            midFilter?.SetPeakingEq(inputFormat.SampleRate, midBand.frequency, midBand.bandwidth, midBand.Gain);
            highFilter?.SetPeakingEq(inputFormat.SampleRate, highBand.frequency, highBand.bandwidth, highBand.Gain);
        }
    }


    public class Band
    {
        private readonly Equaliser parentEqualiser;
        private float gain;

        public readonly float frequency;
        public readonly float bandwidth;
        public float Gain { get => gain; set { gain = value; parentEqualiser.UpdateFilters(); } }

        public Band(Equaliser parentEqualiser, float frequency, float bandwidth)
        {
            this.parentEqualiser = parentEqualiser;
            this.frequency = frequency;
            this.bandwidth = bandwidth;
        }
    }
}
