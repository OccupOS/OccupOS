<<<<<<< HEAD
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArduinoAmbientLightSensor.cs" company="UCL">
//   
// </copyright>
// <summary>
//   Defines the LightSensor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
=======
using Microsoft.SPOT.Hardware;
using IndianaJones.NETMF.Json;
using OccupOS.CommonLibrary.Sensors;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System;
using System.Collections;
>>>>>>> 10a7ddd3b11942d16d26ea6e8d3207df11ed6fc6

namespace OccupOSNode.Micro.Sensors.Arduino {
    using System;
    using System.Collections;

    using IndianaJones.NETMF.Json;

    using Microsoft.SPOT.Hardware;

    using OccupOS.CommonLibrary.Sensors;

    using SecretLabs.NETMF.Hardware.Netduino;

    class LightSensor : Sensor, ILightSensor {
        private AnalogInput input;
        private Hashtable ports = new Hashtable();
        private float analogValue, digitalValue;

        public LightSensor(String id, int portNumber) : base(id) {
            Setup();

            if (portNumber > 0 && portNumber < 6) {
                input = new AnalogInput((Cpu.AnalogChannel)ports[portNumber]);
            } else {
                input = new AnalogInput((Cpu.AnalogChannel)ports[0]);
            }
        }

        public override string GetDataAsJSON() {
            var sensorData = new SensorData {
                AnalogLight = GetAnalogLightValue()
            };

            var jsonSerializer = new Serializer();
            return jsonSerializer.Serialize(sensorData);
        }

        public float GetAnalogLightValue() {
            digitalValue = (float)input.Read();
            analogValue = (float)(digitalValue / 1023 * 3.3);
            return analogValue;
        }

        private void Setup() {
            ports.Add(0, Pins.GPIO_PIN_A0);
            ports.Add(1, Pins.GPIO_PIN_A1);
            ports.Add(2, Pins.GPIO_PIN_A2);
            ports.Add(3, Pins.GPIO_PIN_A3);
            ports.Add(4, Pins.GPIO_PIN_A4);
            ports.Add(5, Pins.GPIO_PIN_A5);
        }
    }
}