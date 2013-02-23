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
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);

            timer.Start();

            Debug.Print("Finished setup");
        }

        void timer_Tick(GT.Timer timer)
        {
            Debug.Print("Taking picture...");
            camera.TakePicture();
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      