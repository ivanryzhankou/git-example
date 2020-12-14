using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace lab_02.DataLayer
{
    internal class BinaryDataRepository
    {
        internal void SerializeFileMetaInformation(Dictionary<string, Models.FileMetaInformation> metaInformationFiles)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("MetaInformationFiles.dat", FileMode.Create))
            {
                formatter.Serialize(fs, metaInformationFiles);
            }
        }

        internal Dictionary<string, Models.FileMetaInformation> DeserializeFileMetaInformation()
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
