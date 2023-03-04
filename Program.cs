using System;
using System.Drawing;
using System.Collections.Generic;
using Newtonsoft.Json;
using TimHanewich.Toolkit;

namespace Mp4Encoding
{
    public class Program
    {
        public static void Main(string[] args)
        {
            byte[] data = RandomBytes(2764800);
            Console.WriteLine(data.Length.ToString("#,##0"));
            Console.ReadLine();

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


        //Converts 2,764,800 bytes or less to a bitmap (2,764,800 because we are targeting a 1280x720 resolution and each pixel can hold 3 bytes (R, G, B))
        public static Bitmap BytesToBitmap(byte[] bytes)
        {


            //If the number of bytes supplied exceeds the limit, throw an error
            if (bytes.Length > 2765800)
            {
                throw new Exception("The maximum number of bytes that can be converted into a single 1280x720 bitmap is 921,600. You supplied " + bytes.Length.ToString("#,##0") + ".");
            }

            Bitmap ToReturn = new Bitmap(1280, 720);
            ByteArrayManager bam = new ByteArrayManager(bytes);
            
            //Write each byte
            int x = 0;
            int y = 0;
            bool ByteArrayManagerHasMore = true;
            while ((x == 0 && y == 720) == false)
            {
                
                //Gather 3 colors
                List<byte> NextColor = new List<byte>();
                while (NextColor.Count < 3 && ByteArrayManagerHasMore)
                {
                    try
                    {
                        byte b = bam.NextByte();
                        NextColor.Add(b);
                    }
                    catch
                    {
                        ByteArrayManagerHasMore = false;
                    }
                }

                //If there are less than three colors, keep adding 0 for the remaining colors until
                while (NextColor.Count < 3)
                {
                    NextColor.Add(0);
                }

                //Set the pixel
                ToReturn.SetPixel(x, y, Color.FromArgb(Convert.ToInt32(NextColor[0]), Convert.ToInt32(NextColor[1]), Convert.ToInt32(NextColor[2])));
                

                //Increment position in bitmap
                if (x == 1279)
                {
                    y = y + 1;
                    x = 0;
                }
                else
                {
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