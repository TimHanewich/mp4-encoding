using System;
using System.Drawing;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mp4Encoding
{
    public class Program
    {
        public static void Main(string[] args)
        {
            byte[] data = RandomBytes(1280);
            Bitmap bm = BytesToBitmap(data);
            bm.Save(@"C:\Users\timh\Downloads\tah\mp4-encoding\output.png");
        }

        public static void Encode(string file_path, string output_dir)
        {
            //Open the file
            FileStream fs = System.IO.File.Open(file_path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);



            //Parse
            while (true)
            {

                //Read the next byte and cancel out if it was -1 (end of stream)
                int next_byte = sr.Read();
                if (next_byte == -1)
                {
                    break;
                }




            }
        }


        //Converts 921,600 bytes or less to a bitmap (921,600 because we are targeting a 1280x720 resolution)
        public static Bitmap BytesToBitmap(byte[] bytes)
        {

            //If the number of bytes supplied exceeds 921,600, throw an error
            if (bytes.Length > 921600)
            {
                throw new Exception("The maximum number of bytes that can be converted into a single 1280x720 bitmap is 921,600. You supplied " + bytes.Length.ToString("#,##0") + ".");
            }

            Bitmap ToReturn = new Bitmap(1280, 720);
            
            //Write each byte
            int x = 0;
            int y = 0;
            for (int b = 0; b < bytes.Length; b++)
            {
                
                int b_ = Convert.ToInt32(bytes[b]);
                ToReturn.SetPixel(x, y, Color.FromArgb(b_, b_, b_));
                
                //Move x and y if needed
                if (x == 1279)
                {
                    Console.WriteLine("Going down a line");
                    y = y + 1; //Go down a row
                    x = 0; //Go back to the start (left side)
                }
                else //Increment one over
                {
                    Console.WriteLine("Going over one");
                    x = x + 1;
                }
            }

            

            return ToReturn;
        }

        # region "toolkit"

        public static byte[] RandomBytes(int count)
        {
            Random r = new Random();
            List<byte> ToReturn = new List<byte>();
            for (int t = 0; t < count; t++)
            {
                byte b = Convert.ToByte(r.Next(255));
                ToReturn.Add(b);
            }
            return ToReturn.ToArray();
        }

        #endregion
    }
}