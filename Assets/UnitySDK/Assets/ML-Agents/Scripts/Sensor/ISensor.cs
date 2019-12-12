namespace MLAgents.Sensor
{
    public enum SensorCompressionType
    {
        None,
        PNG
    }

    /// <summary>
    /// Sensor interface for generating observations.
    /// For custom implementations, it is recommended to SensorBase instead.
    /// </summary>
    public interface ISensor {
        /// <summary>
        /// Returns the size of the observations that will be generated.
        /// For example, a sensor that observes the velocity of a rigid body (in 3D) would return new {3}.
        /// A sensor that returns an RGB image would return new [] {Width, Height, 3}
        /// </summary>
        /// <returns></returns>
        int[] GetFloatObservationShape();

        /// <summary>
        /// Update any internal state of the sensor. This is called once per each agent step.
        /// </summary>
        void Update();

        /// <summary>
        /// Return the compression type being used. If no compression is used, return SensorCompressionType.None
        /// </summary>
        /// <returns></returns>
        SensorCompressionType GetCompressionType();

        /// <summary>
        /// Get the name of the sensor. This is used to ensure deterministic sorting of the sensors on an Agent,
        /// so the naming must be consistent across all sensors and agents.
        /// </summary>
        /// <returns>The name of the sensor</returns>
        string GetName();
    }

    public static class SensorExtensions
    {
        /// <summary>
        /// Get the total number of elements in the ISensor's observation (i.e. the product of the shape elements).
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static int ObservationSize(this ISensor sensor)
        {
            var shape = sensor.GetFloatObservationShape();
            int count = 1;
            for (var i = 0; i < shape.Length; i++)
            {
                count *= shape[i];
            }

            return count;
        }
    }

}
