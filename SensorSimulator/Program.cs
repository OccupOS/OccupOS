using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccupOSCloud
{
    class Program
    {    
        static void Main(string[] args)
        {
            SQLServerHelper helper;
            DateTime now = DateTime.Parse("2013-04-13 01:05:22.0000000");
            int data;
            int count = 10;
            helper = new SQLServerHelper("tcp:dndo40zalb.database.windows.net,1433", "comp2014@dndo40zalb", "20041908kjH", "TestSQLDB");
            while (true)
            {
                data = 40 + new Random().Next(5);
               // if (helper.InsertSensorData(1, 1, (data).ToString(), now.AddSeconds(count), 3) > 0)
                if(helper.InsertSensorData(1,1,(data).ToString(),DateTime.Now,3)>0)
                    System.Diagnostics.Debug.WriteLine("Light Data is inserted");
                else
                    System.Diagnostics.Debug.WriteLine("There was a problem inserting Light Data");
                System.Diagnostics.Debug.WriteLine(data);
                data = 20 + new Random().Next(10);
                //if (helper.InsertSensorData(1, 1, (data).ToString(), now.AddSeconds(count), 9) > 0)
                if (helper.InsertSensorData(1, 1, (data).ToString(), DateTime.Now, 9) > 0)
                    System.Diagnostics.Debug.WriteLine("Temperature Data is inserted");
                else
                    System.Diagnostics.Debug.WriteLine("There was a problem inserting Temperature data");
                count += 10;
                System.Diagnostics.Debug.WriteLine(data);
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
