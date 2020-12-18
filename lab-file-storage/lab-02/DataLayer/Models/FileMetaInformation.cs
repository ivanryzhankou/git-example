﻿using System;

namespace lab_02.DataLayer.Models
{
    [Serializable]
    public class FileMetaInformation
    {
        internal string name = string.Empty;
        internal string extension = string.Empty;
        internal long size;
        internal string creationDate;
        internal int downloadСounter = 0;
        internal string hashChecksum = string.Empty;
    }
}
