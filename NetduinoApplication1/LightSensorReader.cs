using System;
using Microsoft.SPOT;
using System.Collections;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace FirstExample
{
   public class LightSensorReader
    {
       AnalogInput input;
       Hashtable ports = new Hashtable();
       float analogValue, digitalValue;
       public LightSensorReader(int portNumber)
       {
           setup();
           if (portNumber > 0 && portNumber < 6)
           {
               input = new AnalogInput((Cpu.AnalogChannel)ports[portNumber]);
           }
           else
           {
               input = new AnalogInput((Cpu.AnalogChannel)ports[0]);
           }

       }

       public float readValue()
       {
           digitalValue = (float)input.Read();
           analogValue = (float)(digitalValue / 1023 * 3.3);
           return analogValue;
       }
       private void setup()
       {
           ports.Add(0, Pins.GPIO_PIN_A0);
           ports.Add(1, Pins.GPIO_PIN_A1);
           ports.Add(2, Pins.GPIO_PIN_A2);
           ports.Add(3, Pins.GPIO_PIN_A3);
           ports.Add(4, Pins.GPIO_PIN_A4);
           ports.Add(5, Pins.GPIO_PIN_A5);
       }
    }
}
