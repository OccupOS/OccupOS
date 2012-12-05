using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace KinectDemo {

    public partial class MainWindow : Window {
        KinectSensor ksensor;
        WriteableBitmap wbmpCanvas = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            KSChooser.KinectSensorChanged += new DependencyPropertyChangedEventHandler(KSChooser_KinectSensorChanged);
        }

        void KSChooser_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e) {
            KinectSensor old = (KinectSensor)e.OldValue;
            StopKinect(old);
            ksensor = (KinectSensor)e.NewValue;
            ksensor.ColorStream.Enable();
            ksensor.DepthStream.Enable();
            ksensor.SkeletonStream.Enable();
            ksensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(ksensor_AllFramesReady);
            try {
                ksensor.Start();
            }
            catch (System.IO.IOException) {
                KSChooser.AppConflictOccurred();
            }
        }

        void ksensor_AllFramesReady(object sender, AllFramesReadyEventArgs e) {
            if (wbmpCanvas == null) {
                try {
                    wbmpCanvas = new WriteableBitmap(e.OpenDepthImageFrame().Width,
                        e.OpenDepthImageFrame().Height, 96, 96, PixelFormats.Bgr32, null);
                    //Bgr32a => alpha transparency instead of empty byte
                    ImageFrame.Source = wbmpCanvas;
                }
                catch (System.NullReferenceException) {
                    return;
                }
            }
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {
                if (depthFrame == null) {
                    return;
                }
                byte[] pixels = ColourPlayerBytes(depthFrame);
                int stride = 4 * depthFrame.Width;
                wbmpCanvas.WritePixels(new Int32Rect(0, 0, wbmpCanvas.PixelWidth,
                    wbmpCanvas.PixelHeight), pixels, stride, 0);
            }
        }

        Byte[] ColourPlayerBytes(DepthImageFrame depthFrame) {
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            int playerCount = 0;
            bool[] totalPlayers = new bool[7];
            for (int k = 0; k < totalPlayers.Length; k++) {
                totalPlayers[k] = false;
            }

            short[] depthData = new short[depthFrame.PixelDataLength];
            depthFrame.CopyPixelDataTo(depthData);
            Byte[] pixels = new byte[4 * depthFrame.Width * depthFrame.Height];

            for (int depthIndex = 0, colourIndex = 0;
                depthIndex < depthData.Length && colourIndex < pixels.Length;
                depthIndex++, colourIndex += 4) {
                    int player = depthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                    //int depth = depthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                    if (player > 0) {
                        if (totalPlayers[player] == false) totalPlayers[player] = true;
                        pixels[colourIndex + BlueIndex] = 0;
                        pixels[colourIndex + GreenIndex] = 0;
                        pixels[colourIndex + RedIndex] = 255;
                    }
                    else {
                        pixels[colourIndex + BlueIndex] = 0;
                        pixels[colourIndex + GreenIndex] = 0;
                        pixels[colourIndex + RedIndex] = 0;
                    }

            }
            PlayersFoundBox.Clear();
            for (int k = 0; k < totalPlayers.Length; k++) {
                if (totalPlayers[k] == true) playerCount++;
            }
            PlayersFoundBox.AppendText(""+playerCount);
            return pixels;
        }

        void StopKinect(KinectSensor sensor) {
            if (sensor != null) {
                sensor.Stop();
            }
        }

        private void Closing(object sender, EventArgs e) {
            StopKinect(ksensor);
        }
    }
}
