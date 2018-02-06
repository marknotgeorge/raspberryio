namespace Unosquare.RaspberryIO.Components
{
    /// <summary>
    /// Represents a standard 180-degree servo connected to a <see cref="Pca9685Controller"/> <see cref="PwmChannel"/>
    /// </summary>
    public class Servo : PwmChannel
    {
        private int _angle;
        private int _maximum;
        private int _maxAngle;
        private bool _ignoreLimits;
        private int _minimum;
        private int _pulseWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Servo"/> class.
        /// </summary>
        /// <param name="channel">The <see cref="PwmChannel"/> the servo is connected to.</param>
        /// <param name="controller">The <see cref="Pca9685Controller"/> for the channel</param>
        /// <param name="minimum">The servo's minimum pulse width, in microseconds. Defaults to 1000.</param>
        /// <param name="maximum">The servo's maximum pulse width, in microseconds. Defaults to 2000.</param>
        /// <param name="maxAngle">The maximum deflection of the servo, in degrees. Defaults to 180.</param>
        /// <param name="ignoreLimits">Allows the limits to be ignored for testing and calibration purposes. Defaults to false.</param>
        public Servo(Pca9685Controller controller, int channel, int minimum = 1000, int maximum = 2000, int maxAngle = 180, bool ignoreLimits = false)
            : base(channel, controller)
        {
            _minimum = minimum;
            _maximum = maximum;
            _maxAngle = maxAngle;
            _ignoreLimits = ignoreLimits;
        }

        /// <summary>
        /// The angle of the servo.
        /// </summary>
        public int Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                if (!_ignoreLimits)
                {
                    if (value < 0)
                        _angle = 0;
                    else if (value > _maxAngle)
                        _angle = _maxAngle;
                    else _angle = value; 
                }

                var pulseWidth = _minimum + ((_angle * (_maximum - _minimum)) / _maxAngle);
                PulseWidth = pulseWidth;
            }
        }

        /// <summary>
        /// The pulse width, in microseconds.
        /// </summary>
        public int PulseWidth
        {
            get
            {
                return _pulseWidth;
            }

            set
            {
                if (!_ignoreLimits)
                {
                    if (value < _minimum)
                        _pulseWidth = _minimum;
                    else if (value > _maximum)
                        _pulseWidth = _maximum;
                    else
                        _pulseWidth = value; 
                }

                // Calculate the step length in microseconds from the controller's frequency.
                var stepLength = ((1 / Controller.PwmFrequency) * 1_000_000) / 4096;

                // Calculate the number of steps for the pulseWidth.
                var pulseSteps = _pulseWidth / stepLength;

                // Set the channel.
                this.SetPwm(0, pulseSteps);
            }
        }
    }
}