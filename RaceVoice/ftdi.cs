using FTD2XX_NET;
using System;

namespace RaceVoice
{
    public class ftdi
    {
        public string GetSerialNumber(string com)
        {
            string sn = "";
            UInt32 ftdiDeviceCount = 0;
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();

            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                globals.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
                globals.WriteLine("");
            }
            else
            {
                // Wait for a key press
                globals.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                return sn;
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                // Wait for a key press
                globals.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                return sn;
            }

            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            // Populate our device list
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (UInt32 i = 0; i < ftdiDeviceCount; i++)
                {
                    string unitcom = "";
                    string unitsn = "";
                    myFtdiDevice.OpenByIndex(i);
                    myFtdiDevice.GetCOMPort(out unitcom);
                    myFtdiDevice.Close();

                    unitsn = ftdiDeviceList[i].SerialNumber.ToString();

                    globals.WriteLine("Device Index: " + i.ToString());
                    globals.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
                    globals.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
                    globals.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
                    globals.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
                    globals.WriteLine("Serial Number: " + unitsn);
                    globals.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
                    globals.WriteLine("ComPort: " + unitcom);
                    globals.WriteLine("");

                    if (unitcom.ToUpper() == com.ToUpper())
                    {
                        sn = unitsn.ToUpper().Trim();
                    }
                }
            }

            return sn;
        }
    }
}