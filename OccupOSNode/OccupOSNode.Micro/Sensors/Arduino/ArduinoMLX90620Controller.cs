using System;
using Microsoft.SPOT;

namespace OccupOSNode.Sensors.Arduino {

    public static class ArduinoMLX90620Controller {

        public static int freq = 16; //Set this value to your desired refresh frequency

        public static int[] IRDATA = new int[64];
        public static byte CFG_LSB;
        public static byte CFG_MSB;
        public static byte PTAT_LSB;
        public static byte PTAT_MSB;
        public static byte CPIX_LSB;
        public static byte CPIX_MSB;
        public static byte PIX_LSB;
        public static byte PIX_MSB;
        public static int PIX;
        public static int v_th;
        public static int CPIX;
        public static float ta;
        public static float to;
        public static float emissivity;
        public static float k_t1;
        public static float k_t2;
        public static float[] temperatures = new float[64];
        public static int count = 0;
        public static uint PTAT;
        public static int a_cp;
        public static int b_cp;
        public static int tgc;
        public static int b_i_scale;

        public static int[] a_ij = new int[64];
        public static int[] b_ij = new int[64];
        //float alpha_ij[64] = {1.591E-8, 1.736E-8, 1.736E-8, 1.620E-8, 1.783E-8, 1.818E-8, 1.992E-8, 1.748E-8, 1.864E-8, 2.056E-8, 2.132E-8, 2.033E-8, 2.097E-8, 2.324E-8, 2.388E-8, 2.161E-8, 2.155E-8, 2.394E-8, 2.353E-8, 2.068E-8, 2.353E-8, 2.633E-8, 2.708E-8, 2.394E-8, 2.499E-8, 2.778E-8, 2.731E-8, 2.580E-8, 2.539E-8, 2.796E-8, 2.871E-8, 2.598E-8, 2.586E-8, 2.801E-8, 2.830E-8, 2.633E-8, 2.609E-8, 2.894E-8, 2.924E-8, 2.633E-8, 2.464E-8, 2.778E-8, 2.894E-8, 2.673E-8, 2.475E-8, 2.737E-8, 2.796E-8, 2.679E-8, 2.394E-8, 2.708E-8, 2.714E-8, 2.644E-8, 2.347E-8, 2.563E-8, 2.493E-8, 2.388E-8, 2.179E-8, 2.440E-8, 2.504E-8, 2.295E-8, 2.033E-8, 2.283E-8, 2.295E-8, 2.155E-8};  //<-- REPLACE THIS VALUES WITH YOUR OWN!
        //float v_ir_off_comp[64];  //I'm going to merge v_ir_off_comp calculation into v_ir_tgc_comp equation. It's not required anywhere else, so I'll save 256 bytes of SRAM doing this.
        public static float[] v_ir_tgc_comp = new float[64];
        //float v_ir_comp[64];		//removed to save SRAM, in my case v_ir_comp == v_ir_tgc_comp



        public static void config_MLX90620_Hz(int Hz) {
            byte Hz_LSB;
            switch (Hz) {
                case 0:
                    Hz_LSB = B00001111;
                    break;
                case 1:
                    Hz_LSB = B00001110;
                    break;
                case 2:
                    Hz_LSB = B00001101;
                    break;
                case 4:
                    Hz_LSB = B00001100;
                    break;
                case 8:
                    Hz_LSB = B00001011;
                    break;
                case 16:
                    Hz_LSB = B00001010;
                    break;
                case 32:
                    Hz_LSB = B00001001;
                    break;
                default:
                    Hz_LSB = B00001110;
                    break;
            }
            i2c_start_wait(0xC0);
            i2c_write(0x03);
            i2c_write((byte)Hz_LSB - 0x55);
            i2c_write(Hz_LSB);
            i2c_write(0x1F);
            i2c_write(0x74);
            i2c_stop();
        }

        public static void read_EEPROM_MLX90620() {
            byte[] EEPROM_DATA = new byte[256];
            i2c_start_wait(0xA0);
            i2c_write(0x00);
            i2c_rep_start(0xA1);
            for (int i = 0; i <= 255; i++) {
                EEPROM_DATA[i] = i2c_readAck();
            }
            i2c_stop();
            GlobalMembersIrcontrol.varInitialization(EEPROM_DATA);
            GlobalMembersIrcontrol.write_trimming_value(EEPROM_DATA[247]);
        }

        public static void write_trimming_value(byte val) {
            i2c_start_wait(0xC0);
            i2c_write(0x04);
            i2c_write((byte)val - 0xAA);
            i2c_write(val);
            i2c_write(0x56);
            i2c_write(0x00);
            i2c_stop();
        }

        public static void calculate_TA() {
            ta = (-k_t1 + Math.Sqrt(square(k_t1) - (4 * k_t2 * (v_th - (float)PTAT)))) / (2 * k_t2) + 25; //it's much more simple now, isn't it? :)
        }

        public static void calculate_TO() {
            float v_cp_off_comp = (float)CPIX - (a_cp + (b_cp / Math.Pow(2, b_i_scale)) * (ta - 25)); //this is needed only during the to calculation, so I declare it here.

            for (int i = 0; i < 64; i++) {
                v_ir_tgc_comp[i] = IRDATA[i] - (a_ij[i] + (float)(b_ij[i] / Math.Pow(2, b_i_scale)) * (ta - 25)) - (((float)tgc / 32) * v_cp_off_comp);
                //v_ir_comp[i]= v_ir_tgc_comp[i] / emissivity;									//removed to save SRAM, since emissivity in my case is equal to 1. 
                //temperatures[i] = sqrt(sqrt((v_ir_comp[i]/alpha_ij[i]) + pow((ta + 273.15),4))) - 273.15;
                temperatures[i] = Math.Sqrt(Math.Sqrt((v_ir_tgc_comp[i] / alpha_ij[i]) + Math.Pow((ta + 273.15), 4))) - 273.15; //edited to work with v_ir_tgc_comp instead of v_ir_comp
            }
        }


        public static void read_IR_ALL_MLX90620() {
            i2c_start_wait(0xC0);
            i2c_write(0x02);
            i2c_write(0x00);
            i2c_write(0x01);
            i2c_write(0x40);
            i2c_rep_start(0xC1);
            for (int i = 0; i <= 63; i++) {
                PIX_LSB = i2c_readAck();
                PIX_MSB = i2c_readAck();
                IRDATA[i] = (PIX_MSB << 8) + PIX_LSB;
            }
            i2c_stop();
        }

        public static void read_PTAT_Reg_MLX90620() {
            i2c_start_wait(0xC0);
            i2c_write(0x02);
            i2c_write(0x90);
            i2c_write(0x00);
            i2c_write(0x01);
            i2c_rep_start(0xC1);
            PTAT_LSB = i2c_readAck();
            PTAT_MSB = i2c_readAck();
            i2c_stop();
            PTAT = ((uint)PTAT_MSB << 8) + PTAT_LSB;
        }

        public static void read_CPIX_Reg_MLX90620() {
            i2c_start_wait(0xC0);
            i2c_write(0x02);
            i2c_write(0x91);
            i2c_write(0x00);
            i2c_write(0x01);
            i2c_rep_start(0xC1);
            CPIX_LSB = i2c_readAck();
            CPIX_MSB = i2c_readAck();
            i2c_stop();
            CPIX = (CPIX_MSB << 8) + CPIX_LSB;
        }

        public static void read_Config_Reg_MLX90620() {
            i2c_start_wait(0xC0);
            i2c_write(0x02);
            i2c_write(0x92);
            i2c_write(0x00);
            i2c_write(0x01);
            i2c_rep_start(0xC1);
            CFG_LSB = i2c_readAck();
            CFG_MSB = i2c_readAck();
            i2c_stop();
        }

        public static void check_Config_Reg_MLX90620() {
            GlobalMembersIrcontrol.read_Config_Reg_MLX90620();
            if ((CFG_MSB == 0 & 0x04) == 0x04) {
                GlobalMembersIrcontrol.config_MLX90620_Hz(freq);
            }
        }

        public static void varInitialization(byte[] EEPROM_DATA) {
            v_th = (EEPROM_DATA[219] << 8) + EEPROM_DATA[218];
            k_t1 = ((EEPROM_DATA[221] << 8) + EEPROM_DATA[220]) / 1024.0;
            k_t2 = ((EEPROM_DATA[223] << 8) + EEPROM_DATA[222]) / 1048576.0;

            a_cp = EEPROM_DATA[212];
            if (a_cp > 127) {
                a_cp = a_cp - 256;
            }
            b_cp = EEPROM_DATA[213];
            if (b_cp > 127) {
                b_cp = b_cp - 256;
            }
            tgc = EEPROM_DATA[216];
            if (tgc > 127) {
                tgc = tgc - 256;
            }

            b_i_scale = EEPROM_DATA[217];

            emissivity = (((uint)EEPROM_DATA[229] << 8) + EEPROM_DATA[228]) / 32768.0;

            for (int i = 0; i <= 63; i++) {
                a_ij[i] = EEPROM_DATA[i];
                if (a_ij[i] > 127) {
                    a_ij[i] = a_ij[i] - 256;
                }
                b_ij[i] = EEPROM_DATA[64 + i];
                if (b_ij[i] > 127) {
                    b_ij[i] = b_ij[i] - 256;
                }
            }
        }

        public static void Temperatures_Serial_Transmit() {
            for (int i = 0; i <= 63; i++) {
                Serial.println(temperatures[i]);
            }
        }

        public static void setup() {
            pinMode(13, OUTPUT);
            Serial.begin(115200);
            i2c_init();
            PORTC = (1 << PORTC4) | (1 << PORTC5);
            System.Threading.Thread.Sleep(5);
            GlobalMembersIrcontrol.read_EEPROM_MLX90620();
            GlobalMembersIrcontrol.config_MLX90620_Hz(freq);
        }

        public static void loop() {
            if (count == 0) { //TA refresh is slower than the pixel readings, I'll read the values and computate them not every loop.
                GlobalMembersIrcontrol.read_PTAT_Reg_MLX90620();
                GlobalMembersIrcontrol.calculate_TA();
                GlobalMembersIrcontrol.check_Config_Reg_MLX90620();
            }
            count++;
            if (count >= 16) {
                count = 0;
            }
            GlobalMembersIrcontrol.read_IR_ALL_MLX90620();
            GlobalMembersIrcontrol.read_CPIX_Reg_MLX90620();
            GlobalMembersIrcontrol.calculate_TO();
            GlobalMembersIrcontrol.Temperatures_Serial_Transmit();
        }
    }
}
