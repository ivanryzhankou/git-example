using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;
using System.ComponentModel.Design;


namespace lab_02.DataLayer
{
    internal class BinaryDataRepository
    {
        internal void SerializeFileMetaInformation(Models.FileMetaInformation fileMetaInformation)
        {
            Dictionary<string, Models.FileMetaInformation> metaInformationFiles = DeserializeFileMetaInformation();

            metaInformationFiles.Add(fileMetaInformation.name, fileMetaInformation);

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
