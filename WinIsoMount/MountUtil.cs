using System;
using System.Linq;
using System.Management;

namespace WinIsoMount
{
    public class MountUtil
    {
        enum StorageType
        {
            Unknown = 0,
            Iso = 1,
            Vhd = 2,
            Vhdx = 3,
            VhdSet = 4
        }

        enum Access
        {
            Unknown = 0,
            ReadWrite = 2,
            ReadOnly = 3
        }

        const string NamespacePath = @"\\.\ROOT\Microsoft\Windows\Storage";

        const string DiskImageClassName = "MSFT_DiskImage";
        const string MountMethodName = "Mount";
        const string DismountMethodName = "Dismount";

        const string VolumeClassName = "MSFT_Volume";
        const string DriveLetterPropertName = "DriveLetter";

        public static string Test()
        {
            return "test success";
        }

        public static string TestMount()
        {
            string path = "D:\\test\\test.iso";
            return "mount result:" + MountDiskImage(path);
        }

        public static char MountDiskImage(String isoPath)
        {
            string path = BuildPath(isoPath);
            ManagementObject result = new ManagementObject(path);
            result.Get();
            ShowProperties(result);
            uint status = (uint)result.InvokeMethod(MountMethodName, new object[] { Access.ReadOnly, false });
            if (status == (uint)ManagementStatus.NoError)
            {
                return GetDriveLetter(result);
            }
            return '\0';
        }

        public static void DismountDiskImage(String isoPath)
        {
            string path = BuildPath(isoPath);
            ManagementObject result = new ManagementObject(path);
            result.Get();
            ShowProperties(result);
            uint status = (uint)result.InvokeMethod(DismountMethodName, null);
        }

        public static void DismountDiskImageByDevice(String devicePath)
        {
            string path = BuildDevicePath(devicePath);
            ManagementObject result = new ManagementObject(path);
            result.Get();
            ShowProperties(result);
            uint status = (uint)result.InvokeMethod(DismountMethodName, null);
        }

        private static string BuildPath(string imagePath)
        {
            string path = $"{NamespacePath}:{DiskImageClassName}.ImagePath='{imagePath}',StorageType={(int)StorageType.Iso}";
            return path;
        }

        private static string BuildDevicePath(string devicePath)
        {
            string path = $"{NamespacePath}:{DiskImageClassName}.DevicePath='{devicePath}',StorageType={(int)StorageType.Iso}";
            return path;
        }

        /// <summary>
        /// 获取驱动号
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="diskImage"></param>
        /// <returns></returns>
        private static Char GetDriveLetter(ManagementObject diskImage)
        {
            Char result = Char.MinValue;
            using (ManagementObjectCollection items = diskImage.GetRelated(VolumeClassName))
            {
                var volume = items.Cast<ManagementObject>().FirstOrDefault();
                if (volume != null)
                {
                    result = (char)volume[DriveLetterPropertName];
                    ShowProperties(volume);
                }
            }
            return result;
        }

        private static void ShowProperties(ManagementObject obj)
        {
            Console.WriteLine();
            Console.WriteLine($"Properties for \"{obj.ToString()}\"");
            var properties = obj.Properties;
            foreach (PropertyData property in properties)
            {
                string value = GetPropertyValueAsString(property.Value);
                Console.WriteLine($"{property.Name}={value} ({property.Type})");
            }
            Console.WriteLine();
        }

        private static string GetPropertyValueAsString(object value)
        {
            string result = (value == null) ? "{Not Set}" : value.ToString();
            return result;
        }
    }
}
