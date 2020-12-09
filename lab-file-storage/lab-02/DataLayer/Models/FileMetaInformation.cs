using System;
using System.Collections.Generic;
using System.Text;

namespace lab_02.DataLayer.Models
{
    [Serializable]
    public class FileMetaInformation
    {
        internal string name = string.Empty;
        internal string extension = string.Empty;
        internal long size;
        internal string creationDate;
        internal int downloadsNumber = 0;
        internal string hashChecksum = string.Empty;
    }
}
