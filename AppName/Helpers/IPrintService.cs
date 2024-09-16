using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

public interface IPrintService
{
    void Print(Stream inputStream, string fileName);

}