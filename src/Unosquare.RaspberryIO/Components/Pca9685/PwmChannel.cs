namespace Unosquare.RaspberryIO.Components
{ 
    using System;

    /// <summary>
    /// Represents a channel on the PCA9685 board, of which there are 16.
    /// </summary>
    public class PwmChannel
    {
        private static readonly int MaxChannelNumber = 15;
        private static readonly int MaxStep = 4095;
        private Pca9685Controller _controller;
        private int m_channelNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="PwmChannel"/> class.
        /// </summary>
        /// <param name="channelNumber">The channel number. Must be in the range 0..15.</param>
        /// <param name="controller">The controller the channel belongs to.</param>
        public PwmChannel(int channelNumber, Pca9685Controller controller)
        {
            if (channelNumber >= 0 && channelNumber <= MaxChannelNumber)
            {
                Channel = channelNumber;
                Controller = controller;
            }
            else
            {
                throw new ArgumentOutOfRangeException("channelNumber", "channelNumber must be in the range 0..15");
            }
        }

        /// <summary>
        /// The channel number, from 0..15.
        /// </summary>
        public int Channel
        {
            get;
        }

        /// <summary>
        /// The PCA9685 controller the <see cref="PwmChannel"/> is connected to.
        /// </summary>
        public Pca9685Controller Controller
        {
            get
            {
                return _controller;
            }

            private set
            {
                _controller = value;
            }
        }

        /// <summary>
        /// Sets the channel fully on or off.
        /// </summary>
        /// <param name="fullOn">If <c>true</c>, sets the channel fully on else sets the channel fully off</param>
        public void SetFull(bool fullOn)
        {
            if (fullOn)
                SetFullOn();
            else
                SetFullOff();

        }

        /// <summary>
        /// Sets the channel's on and off steps.
        /// </summary>
        /// <param name="on">The on step.</param>
        /// <param name="off">The off step.</param>
        public void SetPwm(int on, int off)
        {
            if (on < 0 || on > MaxStep)
                throw new ArgumentOutOfRangeException("on", "on must be in the range 0..4095");

            if (off < 0 || off > MaxStep)
                throw new ArgumentOutOfRangeException("off", "off must be in the range 0..4095");

            Controller.WriteRegister(Register.LED0_ON_L + (4 * m_channelNumber), on & 0xFF);
            Controller.WriteRegister(Register.LED0_ON_H + (4 * m_channelNumber), on >> 8);
            Controller.WriteRegister(Register.LED0_OFF_L + (4 * m_channelNumber), off & 0xFF);
            Controller.WriteRegister(Register.LED0_OFF_H + (4 * m_channelNumber), off >> 8);
        }
        private void SetFullOff()
        {
            Controller.WriteRegister(Register.LED0_ON_H + (4 * m_channelNumber), 0x00);
            Controller.WriteRegister(Register.LED0_OFF_H + (4 * m_channelNumber), 0x10);
        }

        private void SetFullOn()
        {
            Controller.WriteRegister(Register.LED0_ON_H + (4 * m_channelNumber), 0x10);
            Controller.WriteRegister(Register.LED0_OFF_H + (4 * m_channelNumber), 0x00);
        }
    }
}