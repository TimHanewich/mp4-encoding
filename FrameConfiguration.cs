using System;
using System.Drawing;
using TimHanewich.Toolkit;

namespace Mp4Encoding
{
    public class FrameConfiguration
    {
        public int FrameWidth {get; set;}
        public int FrameHeight {get; set;}
        public int CellsHorizontal {get; set;}
        public int CellsVertical {get; set;}
        public byte[] Data {get; set;}
        
        public FrameConfiguration()
        {
            Data = new byte[]{};
        }

        public int CellCount()
        {
            return CellsHorizontal * CellsVertical;
        }

        public bool DataFits()
        {
            int cells = CellCount();
            if (Data.Length > (cells * 3))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public XYPair CellSize()
        {
            //Calculate width & height of cells
            int CellWidth = FrameWidth / CellsHorizontal;
            int CellHeight = FrameHeight / CellsVertical;

            XYPair ToReturn = new XYPair();
            ToReturn.X = CellWidth;
            ToReturn.Y = CellHeight;
            return ToReturn;
        }

        public XYPair[] SelectCellPixels(int cell_index)
        {
            //Make sure they are not asking for a cell number that is beyond the limits
            if (cell_index > (CellCount() - 1))
            {
                throw new Exception("Cell index '" + cell_index.ToString() + "' is beyond the limits of the current frame configuration.");
            }

            int PixelsToStep = CellSize().X * cell_index; //How many pixels to "step" to the right. Now keep stepping until we get to the starting location!

            int x_start = 0;
            int y_start = 0;
            int x_stop = 0;
            int y_stop = 0;
            while (PixelsToStep > 0)
            {
                x_start = x_start + 1;
                PixelsToStep = PixelsToStep - 1;

                //Re-position if needed
                if (x_start > (FrameWidth - 1))
                {
                    x_start = 0;
                    y_start = y_start + CellSize().Y;
                }
            }

            //Now add to the x_stop and y_stop
            x_stop = x_start + CellSize().X;
            y_stop = y_start + CellSize().Y;
            //Console.WriteLine(x_start.ToString() + " --> " + x_stop.ToString());
            //Console.WriteLine(y_start.ToString() + " --> " + y_stop.ToString());

            //Gather all of the pixels
            List<XYPair> ToReturn = new List<XYPair>();
            for (int y = y_start; y < y_stop; y++)
            {
                for (int x = x_start; x < x_stop; x++)
                {
                    XYPair ToAdd = new XYPair();
                    ToAdd.X = x;
                    ToAdd.Y = y;
                    ToReturn.Add(ToAdd);
                }
            }
            return ToReturn.ToArray();
        }

        public Bitmap ToBitmap()
        {

            //Make sure all of the data fits in the configuration
            bool fits = DataFits();
            if (fits == false)
            {
                throw new Exception("Unable to create bitmap with the current configuration: number of cells is insufficient.");
            }

            Bitmap ToReturn = new Bitmap(FrameWidth, FrameHeight);

            ByteArrayManager bam = new ByteArrayManager(Data);
            bool BytesEmpty = false;
            int on_cell = 0;
            while (BytesEmpty == false)
            {
                //Select until we cannot anymore
                List<byte> rgb = new List<byte>();
                while (rgb.Count < 3 && (BytesEmpty == false))
                {
                    try
                    {
                        byte b = bam.NextByte();
                        rgb.Add(b);
                    }
                    catch
                    {
                        BytesEmpty = true;
                    }
                }

                //Only proceed if there is at least SOME data to keep going with... something worth writing.
                if (rgb.Count > 0)
                {
                    //If it is still not full (3 colors), fill it in with black (0)
                    while (rgb.Count < 3)
                    {
                        rgb.Add(0);
                    }

                    //Get the color
                    Color c = Color.FromArgb(Convert.ToInt32(rgb[0]), Convert.ToInt32(rgb[1]), Convert.ToInt32(rgb[2]));
                
                    XYPair[] pixels = SelectCellPixels(on_cell);
                    foreach (XYPair xy in pixels)
                    {
                        ToReturn.SetPixel(xy.X, xy.Y, c);
                    }
                    on_cell = on_cell + 1;
                }

                
            }


            return ToReturn;
        }
    }
}