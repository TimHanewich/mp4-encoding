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
            //Encode(@"C:\Users\timh\Downloads\tah\mp4-encoding\test.mp4", @"C:\Users\timh\Downloads\tah\mp4-encoding\output");
        }

        # region "encoding"

        public static void Encode(string file_path, string output_dir)
        {
            //Open the file
            FileStream fs = System.IO.File.Open(file_path, FileMode.Open);


            //Write main content
            int last_slide_contains_bytes = 0;
            int on_img = 1;
            List<byte> hopper = new List<byte>();
            bool StreamEmpty = false;
            while (StreamEmpty == false)
            {
                int val = fs.ReadByte();
                if (val == -1)
                {
                    StreamEmpty = true;
                }
                else
                {
                    hopper.Add(Convert.ToByte(val));
                }

                //if the hopper is now full (2,764,800 bytes) OR the stream was finished but we have some to write, generate an image, save it, and clear the hopper
                if (hopper.Count == 2764800 || (StreamEmpty == true && hopper.Count > 0))
                {
                    Bitmap bm = BytesToBitmap(hopper.ToArray());
                    string path = System.IO.Path.Combine(output_dir, on_img.ToString() + ".png");
                    bm.Save(path); //save
                    last_slide_contains_bytes = hopper.Count;
                    hopper.Clear(); //clear the hopper

                    //Increment on_img
                    on_img = on_img + 1;
                }
            }

            //Write the final slide - the info slide
            Bitmap bmi = CreateInfoBitmap(last_slide_contains_bytes);
            string pathi = System.IO.Path.Combine(output_dir, on_img.ToString() + ".png");
            bmi.Save(pathi); //save

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

        public static Bitmap CreateInfoBitmap(int bytes_on_last)
        {
            Bitmap ToReturn = new Bitmap(1280, 720);
            byte[] bytes = BitConverter.GetBytes(bytes_on_last);

            //Add it
            int x = 0;
            foreach (byte b in bytes)
            {
                int i = Convert.ToInt32(b);
                ToReturn.SetPixel(x, 0, Color.FromArgb(i, i, i));
                x = x + 1;
            } 

            return ToReturn;
        }

        # endregion
        
        # region "decoding"

        public static byte[] BitmapToBytes(Bitmap bm, int? bytes_limits = null)
        {
            List<byte> bytes = new List<byte>();

            //Read
            for (int y = 0; y < bm.Height; y++)
            {
                for (int x = 0; x < bm.Width; x++)
                {
                    Color c = bm.GetPixel(x, y);
                    bytes.Add(c.R);
                    bytes.Add(c.G);
                    bytes.Add(c.B);
                }
            }

            //If there was a limit on the number of bytes I am supposed to read from this, take them out one by one
            if (bytes_limits.HasValue)
            {
                while (bytes.Count > bytes_limits.Value)
                {
                    bytes.RemoveAt(bytes.Count - 1); //Trim off the end
                }
            }
            
            return bytes.ToArray();
        }

        public static int DecodeInfoBitmap(Bitmap bm)
        {
            byte b1 = bm.GetPixel(0, 0).R;
            byte b2 = bm.GetPixel(1, 0).R;
            byte b3 = bm.GetPixel(2, 0).R;
            byte b4 = bm.GetPixel(3, 0).R;
            byte[] bytes = new byte[]{b1, b2, b3, b4};

            int ToReturn = BitConverter.ToInt32(bytes, 0);
            return ToReturn;
        }

        # endregion

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