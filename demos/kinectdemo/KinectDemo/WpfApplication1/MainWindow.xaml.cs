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
            var tsparams = new TransformSmoothParameters {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            }; //Delete params when sure not going to use
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
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame()) {
                if (depthFrame == null || skeletonFrame == null) {
                    return;
                }
                byte[] pixels = ColourPlayerBytes(depthFrame);
                int stride = 4 * depthFrame.Width;
                wbmpCanvas.WritePixels(new Int32Rect(0, 0, wbmpCanvas.PixelWidth,
                    wbmpCanvas.PixelHeight), pixels, stride, 0);

                int[] distances = GetPlayerDistances(depthFrame, skeletonFrame);
                int[] actives = { 0, 0 };
                int k = 0;
                foreach (int distance in distances) {
                    if (k > 1) break;
                    if (distance != 0) {
                        actives[k] = distance;
                        k++;
                    }
                }
                    UpdatePlayerCoordinates(actives[0], 0, actives[1], 0);
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
            PlayersFoundBox.AppendText(playerCount.ToString());
            return pixels;
        }

        int[] GetPlayerDistances(DepthImageFrame depthFrame, SkeletonFrame skeletonFrame) {
            Skeleton[] allskeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            int[] distances = new int[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(allskeletons);
            int k = 0;

            foreach (Skeleton skeleton in allskeletons) {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked) {
                    Joint jointpoint = skeleton.Joints[JointType.Head];
                    if (jointpoint.TrackingState == JointTrackingState.Tracked) { //try other point?
                        var depthImagePoint = depthFrame.MapFromSkeletonPoint(jointpoint.Position);
                        distances[k] = depthImagePoint.Depth;
                    }
                    else distances[k] = 0;
                }
                else distances[k] = 0;
                k++;
            }
            return distances;
        }

        void UpdatePlayerCoordinates(double P1X, double P1Y, double P2X, double P2Y) {
            Player1XBox.Clear(); Player1XBox.AppendText(P1X.ToString());
            Player1YBox.Clear(); Player1YBox.AppendText(P1Y.ToString());
            Player2XBox.Clear(); Player2XBox.AppendText(P2X.ToString());
            Player2YBox.Clear(); Player2YBox.AppendText(P2Y.ToString());
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
