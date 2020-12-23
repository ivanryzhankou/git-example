using System;

namespace DataLayer.Models
{
    [Serializable]
    public class FileMetaInformation
    {
        public string name = string.Empty;
        public string extension = string.Empty;
        public long size;
        public string creationDate;
        public int downloadСounter = 0;
        public string hashChecksum = string.Empty;
    }
}
