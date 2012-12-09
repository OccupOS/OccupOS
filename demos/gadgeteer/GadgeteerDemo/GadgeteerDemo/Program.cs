using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerDemo
{
    public partial class Program
    {
        private readonly GT.Timer timer = new GT.Timer(2000);

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);


            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);

            timer.Start();

            Debug.Print("Finished setup");
        }

        void timer_Tick(GT.Timer timer)
        {
            Debug.Print("Taking picture...");
            camera.TakePicture();
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            Debug.Print("Drawing picture...");
            display_T35.SimpleGraphics.Clear();
            display_T35.SimpleGraphics.DisplayImage(picture.MakeBitmap(), 0, 0);
            display_T35.SimpleGraphics.Redraw();
        }
    }
}
