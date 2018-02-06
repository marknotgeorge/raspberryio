namespace Unosquare.RaspberryIO.Components
{
    using System;
    using System.Collections.Generic;
    using Unosquare.RaspberryIO.Gpio;
    using Unosquare.RaspberryIO.Native;

    /// <summary>
    /// Implementation of the PCA9685 12-bit 16-channel PWM controller.
    /// </summary>
    public class Pca9685Controller
    {
        private static readonly uint Delay = 500;
        private static readonly int MaxChannels = 15;
        private static readonly int MaxSteps = 4096;
        private static readonly double Twenty5MHz = 25_000_000.00;
        private int _pwmFrequency;
        private Dictionary<int, PwmChannel> _registeredChannels;
        private I2CDevice m_I2CDevice;
        private GpioPin m_OePin;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Pca9685Controller"/> class.        
        /// </summary>
        /// <param name="oePinNumber">The pin number of the Output Enable pin</param>
        /// <param name="deviceId">The I2C device id of the controller. Defaults to 0x40.</param>
        public Pca9685Controller(int oePinNumber, int deviceId = 0x40)
        {
            m_I2CDevice = Pi.I2C.AddDevice(deviceId);
            m_OePin = Pi.Gpio[oePinNumber];
            m_OePin.PinMode = GpioPinDriveMode.Output;
            _registeredChannels = new Dictionary<int, PwmChannel>();
        }

        /// <summary>
        /// Sets whether the board's output is enabled or not. 
        /// <remarks>
        /// OutputEnable on the PCA9865 board is active low.
        /// </remarks>        
        /// </summary>
        public bool OutputEnable
        {
            get
            {
                var pinValue = m_OePin.ReadValue();
                return (pinValue == GpioPinValue.Low) ? true : false;
            }
            set
            {
                if (value)
                    m_OePin.Write(GpioPinValue.Low);
                else
                    m_OePin.Write(GpioPinValue.High);
            }
        }

        /// <summary>
        /// The PWM update frequency in Hertz.
        /// </summary>        
        public int PwmFrequency
        {
            get
            {
                return _pwmFrequency;
            }
            set
            {
                _pwmFrequency = value;

                var prescaleValue = (int)Math.Round(Twenty5MHz / (MaxSteps * value)) - 1;

                // Put the controller to sleep
                var oldMode = ReadRegister(Register.MODE1);
                var newMode = (byte)((oldMode & 0x7F) | 0x10);
                WriteRegister(Register.MODE1, newMode);

                // Write the prescale value
                WriteRegister(Register.PRESCALE, (byte)prescaleValue);

                // Wait 500 milliseconds
                Timing.SleepMilliseconds(Delay);

                // Restart controller...
                WriteRegister(Register.MODE1, oldMode | 0x80);
            }
        }

        /// <summary>
        /// Gets the specified <see cref="PwmChannel"/>.
        /// </summary>
        /// <param name="channel">The number of the channel.</param>
        /// <returns>The <see cref="PwmChannel"/>.</returns>
        public PwmChannel this[int channel] => _registeredChannels[channel];

        /// <summary>
        /// Gets the specified <see cref="PwmChannel"/>.
        /// </summary>
        /// <param name="channel">The number of the channel.</param>
        /// <returns>The <see cref="PwmChannel"/>.</returns>
        public PwmChannel Channel(int channel)
        {
            if (channel < 0 || channel > MaxChannels)
                throw new ArgumentOutOfRangeException("channel must be between 0 and 15", "channel");

            return _registeredChannels[channel];
        }

        /// <summary>
        /// Drops the specified channel.
        /// </summary>
        /// <param name="channel">The channel to drop.</param>
        public void DropChannel(int channel)
        {
            _registeredChannels[channel] = null;
        }

        /// <summary>
        /// Registers a <see cref="PwmChannel"/> on the <see cref="Pca9685Controller"/>.
        /// 
        /// Throws an <see cref="ArgumentException"/> if the channel is already registered.
        /// </summary>
        /// <param name="channel">The number of the channel to register, in the range 0..15</param>
        /// <returns>A <see cref="PwmChannel"/> object. </returns>
        public PwmChannel RegisterChannel(int channel)
        {
            if (channel < 0 || channel > MaxChannels)
                throw new ArgumentOutOfRangeException("channel must be between 0 and 15", "channel");

            if (_registeredChannels[channel] != null)
                throw new ArgumentException("Channel already registered.", "channel");

            _registeredChannels[channel] = new PwmChannel(channel, this);

            return _registeredChannels[channel];
        }
        internal byte ReadRegister(Register register)
        {
            return m_I2CDevice.ReadAddressByte((int)register);
        }

        internal void WriteRegister(Register register, int data)
        {
            WriteRegister(register, (byte)data);
        }

        internal void WriteRegister(Register register, byte data)
        {
            m_I2CDevice.WriteAddressByte((int)register, data);
        }        
    }
}
