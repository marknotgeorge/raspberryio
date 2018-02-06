namespace Unosquare.RaspberryIO.Components
{
    internal enum Register
    {
        // SUBADR1 = 0x02,
        // SUBADR2 = 0x03,
        // SUBADR3 = 0x04,
        MODE1 = 0x00,
        PRESCALE = 0xFE,
        LED0_ON_L = 0x06,
        LED0_ON_H = 0x07,
        LED0_OFF_L = 0x08,
        LED0_OFF_H = 0x09,

        // ALLLED_ON_L = 0xFA,
        // ALLLED_ON_H = 0xFB,
        // ALLLED_OFF_L = 0xFC,
        // ALLLED_OFF_H = 0xFD,
    }
}
