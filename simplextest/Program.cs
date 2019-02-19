
using System;
using System.Runtime.InteropServices;

using itk.simple;
using PixelId = itk.simple.PixelIDValueEnum;

namespace itk.simple.examples
{
    public class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: SimpleGaussian <input>");
                return;
            }

            // Read input image
            itk.simple.Image input = SimpleITK.ReadImage(args[0]);

            // Cast to we know the the pixel type
            input = SimpleITK.Cast(input, PixelId.sitkVectorFloat32);

            // calculate the number of pixels
            VectorUInt32 size = input.GetSize();
            int len = 1;
            for (int dim = 0; dim < input.GetDimension(); dim++)
            {
                len *= (int)size[dim];
            }
            IntPtr buffer = input.GetBufferAsFloat();

            // There are two ways to access the buffer:

            // (1) Access the underlying buffer as a pointer in an "unsafe" block
            // (note that in C# "unsafe" simply means that the compiler can not
            // perform full type checking), and requires the -unsafe compiler flag
            // unsafe {
            //   float* bufferPtr = (float*)buffer.ToPointer();

            //   // Now the byte pointer can be accessed as per Brad's email
            //   // (of course this example is only a 2d single channel image):
            //   // This is a 1-D array but can be access as a 3-D. Given an
            //   // image of size [xS,yS,zS], you can access the image at
            //   // index [x,y,z] as you wish by image[x+y*xS+z*xS*yS],
            //   // so x is the fastest axis and z is the slowest.
            //   for (int j = 0; j < size[1]; j++) {
            //     for (int i = 0; i < size[0]; i++) {
            //       float pixel = bufferPtr[i + j*size[1]];
            //       // Do something with pixel here
            //     }
            //   }
            // }

            // (2) Copy the buffer to a "safe" array (i.e. a fully typed array)
            // (note that this means memory is duplicated)
            float[] bufferAsArray = new float[len]; // Allocates new memory the size of input
            Marshal.Copy(buffer, bufferAsArray, 0, len);
            double total = 0.0;
            for (int j = 0; j < size[1]; j++)
            {
                for (int i = 0; i < size[0]; i++)
                {
                    float pixel = bufferAsArray[i + j * size[1]];
                    total += pixel;
                }
            }
            Console.WriteLine("Pixel value total: {0}", total);
            Console.ReadKey();
        }
    }
}