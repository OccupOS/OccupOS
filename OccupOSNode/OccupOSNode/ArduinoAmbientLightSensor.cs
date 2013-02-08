using Microsoft.SPOT.Hardware;
using IndianaJones.NETMF.Json;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Collections;


namespace OccupOSNode {
    class LightSensor : Sensor {
        private AnalogInput input;
        private Hashtable ports = new Hashtable();
        private float analogValue, digitalValue;

        public LightSensor(String id, int portNumber) : base(id) {
            setup();

            if (portNumber > 0 && portNumber < 6) {
                input = new AnalogInput((Cpu.AnalogChannel)ports[portNumber]);
            } else {
                input = new AnalogInput((Cpu.AnalogChannel)ports[0]);
            }
        }

        public float readValue() {
            digitalValue = (float)input.Read();
            analogValue = (float)(digitalValue / 1023 * 3.3);
            return analogValue;
        }

        private void setup() {
            ports.Add(0, Pins.GPIO_PIN_A0);
            ports.Add(1, Pins.GPIO_PIN_A1);
            ports.Add(2, Pins.GPIO_PIN_A2);
            ports.Add(3, Pins.GPIO_PIN_A3);
            ports.Add(4, Pins.GPIO_PIN_A4);
            ports.Add(5, Pins.GPIO_PIN_A5);
        }

        public override string poll() {
            float light = readValue();
            model.readingData = light.ToString();
            Serializer jsonSerializer = new Serializer();
            return jsonSerializer.Serialize(model);
        }
    }
}
