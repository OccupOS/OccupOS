using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System;
using System.Threading;

namespace OccupOSNode
{
    public class WeatherShieldController
    {
        public const byte CMD_UNKNOWN = 0x00, CMD_SETADDRESS = 0x01, CMD_ECHO_PAR = 0x02, CMD_SET_SAMPLETIME = 0x03,
                          CMD_GETTEMP_C_AVG = 0x04, CMD_GETTEMP_C_RAW = 0x05, CMD_GETPRESS_AVG = 0x06, CMD_GETPRESS_RAW = 0x07,
                          CMD_GETHUM_AVG = 0x08, CMD_GETHUM_RAW = 0x09, PAR_GET_LAST_SAMPLE = 0x80, PAR_GET_AVG_SAMPLE = 0x81;
        private byte m_clockPin, m_dataPin, m_deviceAddress;
        const int RXCOMMANDPOS=3, RXPAR1POS = 2, RXPAR2POS = 1, RXPAR3POS = 0, RXBUFFERLENGTH = 4, WEATHERSHIELD_DEFAULTIODATA_PIN = 2,
                  WEATHERSHIELD_DEFAULTCLOCK_PIN = 7;
        const byte WEATHERSHIELD_DEFAULTADDRESS = 0x01;
        OutputPort portClock,portData;
        InputPort dataIn;
        
        public WeatherShieldController()
        {
            m_clockPin = WEATHERSHIELD_DEFAULTCLOCK_PIN;
            m_dataPin = WEATHERSHIELD_DEFAULTIODATA_PIN;
            m_deviceAddress = WEATHERSHIELD_DEFAULTADDRESS;
            portClock = new OutputPort(Pins.GPIO_PIN_D7,false);
            portData = new OutputPort(Pins.GPIO_PIN_D2,false);
            dataIn = new InputPort(Pins.GPIO_PIN_D2,true,Port.ResistorMode.Disabled);
            resetConnection();
        }
        
        public WeatherShieldController(byte clockpin, byte datapin, byte deviceaddress)
        {
            m_clockPin = clockpin;
            m_dataPin = datapin;
            m_deviceAddress = deviceaddress;

            /* Start with a reset */
            resetConnection();
        }
        
        /* Send a specific command to the weather shield and return the related
        answer. The answer will be stored in the provided buffer. 
        This function returns true if the operation successfully terminates */
        public bool sendCommand(byte ucCommand, byte ucParameter, ref byte[] pucBuffer)
        {

	        sendCommand(ucCommand, ucParameter);
	        //delayMicroseconds(15000);
            Thread.Sleep(90);

	        bool bResult = readAnswer(ucCommand,ref pucBuffer);
	
	        return bResult;
        }

        /* Decode the float value stored in the buffer */
        public float decodeFloatValue(ref byte[] pucBuffer) 
        {
            byte cMSD = (byte) pucBuffer[RXPAR1POS];
            byte cLSD = (byte) pucBuffer[RXPAR2POS];

            float fVal = cMSD + (((float)cLSD) / 100.0f);
  
            return fVal;
        }

        /* ----------------------------------------------------------------- */

        /* Decode an short value stored in the buffer */
        public ushort decodeShortValue(byte[] pucBuffer) 
        {
	
          byte ucMSD = pucBuffer[RXPAR1POS];
          byte ucLSD = pucBuffer[RXPAR2POS];
	
          ushort shResult = (ushort)(ucMSD << 8 | ucLSD);
	
          return shResult;
        }

        public void decodeFloatAsString(byte[] pucBuffer, ref String chString) {
	
            byte cMSD = (byte)pucBuffer[RXPAR1POS];
            byte cLSD = (byte)pucBuffer[RXPAR2POS];
	
            if (cLSD < 0) {
	            cLSD = (byte)((int)(-cLSD));
	  
	            if (cMSD < 0)
		            cMSD = (byte)((int)(-cMSD));

	            // sprintf(chString,"-%d.%d", cMSD, cLSD);
                chString = "-"+cMSD.ToString()+cLSD.ToString();
            } else
	            chString = cMSD.ToString()+cLSD.ToString();
        }

/* ----------------------------------------------------------------- */

        /* Send a series of low level bits in order to reset
the communication channel between the Arduino and the 
Weather Shield 1 */
void resetConnection() {
  
  /* Clock is always an output pin */
  //pinMode(m_clockPin, OUTPUT);
  //digitalWrite(m_clockPin, LOW); 
    portClock.Write(false);

  /* Set data pin in output mode */
//  pinMode(m_dataPin, OUTPUT);
  
  /* We start sending the first high level bit */
 // digitalWrite(m_dataPin, HIGH);
    portData.Write(true);
  pulseClockPin();
  
  /* Send a sequence of "fake" low level bits */
  for (int ucN = 0; ucN < 200; ucN++) {

   // digitalWrite(m_dataPin, LOW);
      portData.Write(false);
    pulseClockPin();
  }
}

/* ----------------------------------------------------------------- */

/* Generate a clock pulse */
void pulseClockPin() {
  //digitalWrite(m_clockPin, HIGH);
    portClock.Write(true);
  //delayMicroseconds(5000);
    Thread.Sleep(5);
 // digitalWrite(m_clockPin, LOW);  
    portClock.Write(false);     
  //delayMicroseconds(5000);	
    Thread.Sleep(5);
}

/* ----------------------------------------------------------------- */

        /* Send a byte through the communication bus (MSb first) */
void sendByte(byte ucData) {
  
  for (byte ucN = 0; ucN < 8; ucN++) {
    
    if ((ucData & 0x80)>0)
    {
     // digitalWrite(m_dataPin, HIGH);
        portData.Write(true);
    }
    else
    {
     // digitalWrite(m_dataPin, LOW);
        portData.Write(false);
    }
      
    pulseClockPin();
    ucData <<=1;
  }
}

/* ----------------------------------------------------------------- */
byte readByte() {
  
  byte ucResult = 0;
  
  for (byte ucN = 0; ucN < 8; ucN++) {
    
	//digitalWrite(m_clockPin, HIGH);
      portClock.Write(true);
	//delayMicroseconds(5000);
      Thread.Sleep(5);

	ucResult <<= 1;

	  bool ucIn = dataIn.Read();
	if (ucIn)
		ucResult |= 1;


	//digitalWrite(m_clockPin, LOW);
      portClock.Write(false);

	//delayMicroseconds(5000);
      Thread.Sleep(5);
  }
  
  return ucResult;
}

/* ----------------------------------------------------------------- */

        /* Send a command request to the Weather Shield 1 */
void sendCommand(byte ucCommand, byte ucParameter) {
  
  /* Set data pin in output mode */
 // pinMode(m_dataPin, OUTPUT);
  
  /* We start sending the first high level bit */
  //digitalWrite(m_dataPin, HIGH);
    portData.Write(true);
  pulseClockPin();
  
  /* The first byte is always 0xAA... */
  sendByte(0xAA);
	
  /* ... then is the address... */
  sendByte(m_deviceAddress);
	
  /* ... then is the command ... */
  sendByte(ucCommand);
 
  /* ... and the parameter ... */
  sendByte(ucParameter);
  
  /* And this is the last low level bit required by the protocol */
  //digitalWrite(m_dataPin, LOW);
   portData.Write(false);
  pulseClockPin();
}

/* ----------------------------------------------------------------- */
        /* Read the answer back from the Weather Shield 1 and fill the provided
buffer with the result. Depending on the type of command associated
to this answer the buffer contents should be properly decoded.
The function returns true if the read answer contain the expected 
command */
bool readAnswer(byte ucCommand, ref byte[] pucBuffer) {
  
  /* Set data pin in input mode */
 // pinMode(m_dataPin, INPUT);
  
  /* Read RXBUFFERLENGTH bytes from the Weather Shield 1 */
    for (byte ucN = RXBUFFERLENGTH; ucN > 0; ucN--) 
    pucBuffer[ucN-1] = readByte();
  
  /* Set data pin in output mode */
  //pinMode(m_dataPin, OUTPUT);
  
  return (pucBuffer[RXCOMMANDPOS] == ucCommand);
}
}
}
