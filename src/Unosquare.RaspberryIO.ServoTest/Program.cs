

namespace Unosquare.RaspberryIO.ServoTest
{
    using System;
    using Unosquare.RaspberryIO.Components;

    class Program
    {
        static void Main(string[] args)
        {
            var controller = new Pca9685Controller(0);
            var servo = new Servo(controller, 0, ignoreLimits: true);



        }
    }
}
