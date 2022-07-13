using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace PictureAndVideoOrganizer
{
    class Program
    {
        private static Regex r = new Regex(":");

        public static void Main(string[] args)
        {
            //Console.WriteLine("please enter the starting location");
            //var sourcePath = Console.ReadLine();
            //Console.WriteLine("please enter the target location");
            //var destinationPath = Console.ReadLine();

            //var sourcePath = "G:\\Pictures";
            //var sourcePath = "D:\\TempPictures";
            //var destinationPath = "D:\\Pictures";

            string sourcePath;
            string destinationPath;

            try
            {
                sourcePath = args[0];
                destinationPath = args[1];

                if (!Directory.Exists(sourcePath) || !Directory.Exists(destinationPath))
                {
                    throw new ArgumentException("Unable to determine file paths.");
                }


                foreach (string file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
                {
                    string month;
                    string targetPath;
                    DateTime dateTaken;

                    var fi = new FileInfo(file);

                    if (fi.Extension.ToUpper() == ".JPG")
                    {
                        dateTaken = GetDateTakenFromImage(file);
                        month = dateTaken.Month.ToString();
                    }
                    else
                    {
                        dateTaken = fi.LastWriteTime;
                        month = dateTaken.Month.ToString();
                    }

                    if (AddZero(dateTaken.Month))
                    {
                        month = "0" + month;
                    }

                    targetPath = string.Format("{0}\\{1}\\{2}\\", destinationPath, dateTaken.Year, month);

                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                        Console.WriteLine(string.Format("Created Directory : {0}", targetPath));
                    }

                    if (!File.Exists(targetPath + fi.Name))
                    {
                        fi.MoveTo(targetPath + fi.Name);
                        Console.WriteLine(string.Format("File moved : {0} to : {1}", fi.Name, targetPath));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("File {0} exists in target directory.", fi.Name));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey(true);
                return;
            }

            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        public static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An error occured : {0}", e));
            }

            return new DateTime();
        }

        public static bool AddZero(int month)
        {
            return month < 10;
        }
    }
}
