﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unosquare.Swan.Abstractions;
using Unosquare.Swan.Components;

namespace Unosquare.RaspberryIO.Bluetooth
{
    public class BluetoothController : SingletonBase<BluetoothController>
    {
        #region SimpleFunctionBluetooth

        /// <summary>
        /// Retrieves current bluetooth status.
        /// </summary>
        //public string StatusBT() => ProcessRunner.GetProcessOutputAsync("systemctl", "status bluetooth").Result;
        public bool StatusBT()
        {
            var statusOutput = ProcessRunner.GetProcessOutputAsync("systemctl", "status bluetooth").Result;
            return statusOutput.Contains("Running") ? true : false;           
        }

        //public string InitializeBT() => ProcessRunner.GetProcessOutputAsync("sudo", "bluetoothctl").Result;

        #endregion
    }
}
