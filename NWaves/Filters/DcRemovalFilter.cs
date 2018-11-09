using NWaves.Filters.Base;

namespace NWaves.Filters
{
    /// <summary>
    /// DC removal IIR filter
    /// </summary>
    public class DcRemovalFilter : IirFilter
    {
        /// <summary>
        /// Delay line
        /// </summary>
        private float _in1;
        private float _out1;

        /// <summary>
        /// Constructor creates simple 1st order recursive filter
        /// </summary>
        /// <param name="r">R coefficient (usually in [0.9, 1] range)</param>
        public DcRemovalFilter(double r = 0.995) : base(new [] {1, -1.0}, new [] {1, -r})
        {
        }

        /// <summary>
        /// Online filtering
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filteringOptions"></param>
        /// <returns></returns>
        public override float[] Process(float[] input, FilteringOptions filteringOptions = FilteringOptions.Auto)
        {
            var output = new float[input.Length];

            var b = _b32;
            var a = _a32;

            for (var n = 0; n < input.Length; n++)
            {
                output[n] = b[0] * input[n] + b[1] * _in1 - a[1] * _out1;
                _in1 = input[n];
                _out1 = output[n];
            }

            return output;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _in1 = _out1 = 0;
        }
    }
}
