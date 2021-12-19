using System;
using System.IO;

namespace BertScout2022.Env
{
    public static class DeviceEnvironment
    {
        public static string GetMyDocumentsPath()
        {
            string myDocumentsPath = "";

            string baseDocumentsPath = "/storage/sdcard0";
            if (!Directory.Exists(baseDocumentsPath))
            {
                baseDocumentsPath = "/storage/sdcard"; // android emulator
            }
            if (Directory.Exists(baseDocumentsPath))
            {
                myDocumentsPath = $"{baseDocumentsPath}/Documents";
                if (!Directory.Exists(myDocumentsPath))
                {
                    try
                    {
                        Directory.CreateDirectory(myDocumentsPath);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            if (!Directory.Exists(myDocumentsPath))
            {
                myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // windows
            }
            if (!Directory.Exists(myDocumentsPath))
            {
                myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // local data
            }
            return myDocumentsPath;
        }

        public static string GetMyPicturesPath()
        {
            string myPicturesPath = "";

            string basePicturesPath = "/storage/sdcard0";
            if (!Directory.Exists(basePicturesPath))
            {
                basePicturesPath = "/storage/sdcard"; // android emulator
            }
            if (Directory.Exists(basePicturesPath))
            {
                myPicturesPath = $"{basePicturesPath}/DCIM/Camera";
                if (!Directory.Exists(myPicturesPath))
                {
                    try
                    {
                        Directory.CreateDirectory(myPicturesPath);
                    }
                    catch
                    {
                        //ignore
                    }
                }
            }
            if (!Directory.Exists(myPicturesPath))
            {
                myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); // windows
            }
            if (!Directory.Exists(myPicturesPath))
            {
                myPicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // local data
            }
            return myPicturesPath;
        }
    }
}
