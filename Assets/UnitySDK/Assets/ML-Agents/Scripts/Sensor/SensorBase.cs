using UnityEngine;

namespace MLAgents.Sensor
{
    public abstract class SensorBase : ISensor
    {
        /// <summary>
        /// Write the observations to the output buffer. This size of the buffer will be product of the sizes returned
        /// by GetFloatObservationShape().
        /// </summary>
        /// <param name="output"></param>
        public abstract void WriteObservation(float[] output);

        public abstract int[] GetFloatObservationShape();

        public abstract string GetName();

        public void Update() { }

        public virtual byte[] GetCompressedObservation()
        {
            return null;
        }

        public virtual SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }
    }
}