using DataLayer.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataLayer
{
    public class BinaryDataRepository : IBinaryDataRepository
    {
        public void SerializeFileMetaInformation(Dictionary<string, Models.FileMetaInformation> metaInformationFiles)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("MetaInformationFiles.dat", FileMode.Create))
            {
                formatter.Serialize(fs, metaInformationFiles);
            }
        }

        public Dictionary<string, Models.FileMetaInformation> DeserializeFileMetaInformation()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("MetaInformationFiles.dat", FileMode.OpenOrCreate))
            {
                Dictionary<string, Models.FileMetaInformation> metaInformationFiles = (Dictionary<string, Models.FileMetaInformation>)formatter.Deserialize(fs);

                return metaInformationFiles;
            }
        }
    }
}
