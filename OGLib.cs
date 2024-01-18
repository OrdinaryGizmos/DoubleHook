using System;
using Godot;
namespace OG
{
    public struct Spring
    {
        public double mass = 0;
        public double k = 0;
        public double damp = 0;
        public double force = 0;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;
        public Vector2 point1 = Vector2.Zero;
        public Vector2 point2 = Vector2.Zero;

        public Spring(double m, double _k, double d)
        {
            mass = m;
            k = _k;
            damp = d;
        }

        public static Vector3 GetForce(double m, double k, double d, Vector3 p1, Vector3 p2, double delta, Vector3 v)
        {
            double force = (p1 - p2).Length() * k;
            Vector3 direction = (p1 - p2).Normalized();
            double acc = force / m;
            Vector3 velocity = v * delta;

            return -force * direction - velocity * d;
        }
    }

    /// <summary>
    /// A (P)roportional, (I)ntegral, (D)erivative Controller
    /// </summary>
    /// <remarks>
    /// The controller should be able to control any process with a
    /// measureable value, a known ideal value and an input to the
    /// process that will affect the measured value.
    /// </remarks>
    /// <see cref="https://en.wikipedia.org/wiki/PID_controller"/>
    public sealed class PidController
    {
        private double processVariable = 0;

        public PidController(double GainProportional, double GainIntegral, double GainDerivative, double OutputMax, double OutputMin)
        {
            this.GainDerivative = GainDerivative;
            this.GainIntegral = GainIntegral;
            this.GainProportional = GainProportional;
            this.OutputMax = OutputMax;
            this.OutputMin = OutputMin;
        }

        public double ControlVariable(double timeSinceLastUpdate){
            return ControlVariable(TimeSpan.FromMicroseconds(timeSinceLastUpdate * 1000000));
        }

        /// <summary>
        /// The controller output
        /// </summary>
        /// <param name="timeSinceLastUpdate">timespan of the elapsed time
        /// since the previous time that ControlVariable was called</param>
        /// <returns>Value of the variable that needs to be controlled</returns>
        public double ControlVariable(TimeSpan timeSinceLastUpdate)
        {
            double error = SetPoint - ProcessVariable;

            // integral term calculation
            IntegralTerm += (GainIntegral * error * timeSinceLastUpdate.TotalSeconds);
            IntegralTerm = Clamp(IntegralTerm);

            // derivative term calculation
            double dInput = processVariable - ProcessVariableLast;
            double derivativeTerm = GainDerivative * (dInput * timeSinceLastUpdate.TotalSeconds);

            // proportional term calcullation
            double proportionalTerm = GainProportional * error;

            double output = proportionalTerm + IntegralTerm - derivativeTerm;

            output = Clamp(output);

            return output;
        }

        /// <summary>
        /// The derivative term is proportional to the rate of
        /// change of the error
        /// </summary>
        public double GainDerivative { get; set; } = 0;

        /// <summary>
        /// The integral term is proportional to both the magnitude
        /// of the error and the duration of the error
        /// </summary>
        public double GainIntegral { get; set; } = 0;

        /// <summary>
        /// The proportional term produces an output value that
        /// is proportional to the current error value
        /// </summary>
        /// <remarks>
        /// Tuning theory and industrial practice indicate that the
        /// proportional term should contribute the bulk of the output change.
        /// </remarks>
        public double GainProportional { get; set; } = 0;

        /// <summary>
        /// The max output value the control device can accept.
        /// </summary>
        public double OutputMax { get; private set; } = 0;

        /// <summary>
        /// The minimum ouput value the control device can accept.
        /// </summary>
        public double OutputMin { get; private set; } = 0;

        /// <summary>
        /// Adjustment made by considering the accumulated error over time
        /// </summary>
        /// <remarks>
        /// An alternative formulation of the integral action, is the
        /// proportional-summation-difference used in discrete-time systems
        /// </remarks>
        public double IntegralTerm { get; private set; } = 0;


        /// <summary>
        /// The current value
        /// </summary>
        public double ProcessVariable
        {
            get { return processVariable; }
            set
            {
                ProcessVariableLast = processVariable;
                processVariable = value;
            }
        }

        /// <summary>
        /// The last reported value (used to calculate the rate of change)
        /// </summary>
        public double ProcessVariableLast { get; private set; } = 0;

        /// <summary>
        /// The desired value
        /// </summary>
        public double SetPoint { get; set; } = 0;

        /// <summary>
        /// The acceptable distance from the SetPoint
        /// </summary>
        public double Error { get; set; } = 0;

        /// <summary>
        /// Limit a variable to the set OutputMax and OutputMin properties
        /// </summary>
        /// <returns>
        /// A value that is between the OutputMax and OutputMin properties
        /// </returns>
        /// <remarks>
        /// Inspiration from http://stackoverflow.com/questions/3176602/how-to-force-a-number-to-be-in-a-range-in-c
        /// </remarks>
        private double Clamp(double variableToClamp)
        {
            if (variableToClamp <= OutputMin) { return OutputMin; }
            if (variableToClamp >= OutputMax) { return OutputMax; }
            return variableToClamp;
        }
    }
}
