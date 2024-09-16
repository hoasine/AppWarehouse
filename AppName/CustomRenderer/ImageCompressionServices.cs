using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace AppName.CustomRenderer
{
    public interface IImageCompressionServices
    {
        byte[] CompressImage(byte[] imageData, string destionationPath, int comperssionPersion);
    }

  
}
