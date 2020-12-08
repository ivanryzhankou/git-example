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
        Models.FileMetaInformation fileMetaInformation = new Models.FileMetaInformation();

        internal void SerializeFileMetaInformation(string fileName, string fileExtension, long fileSize, string fileCreationDate, int fileDownloadsNumber,
            string fileHashСheckSum)
        {
            fileMetaInformation.name = fileName;
            fileMetaInformation.extension = fileExtension;
            fileMetaInformation.size = fileSize;
            fileMetaInformation.creationDate = fileCreationDate;
            fileMetaInformation.downloadsNumber = fileDownloadsNumber;
            fileMetaInformation.hashChecksum = fileHashСheckSum;

            Dictionary<string, Models.FileMetaInformation> metaInformationFiles = DeserializeFileMetaInformation();

            metaInformationFiles.Add(fileName, fileMetaInformation);

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("MetaInformationFiles.dat", FileMode.Create))
            {
                formatter.Serialize(fs, metaInformationFiles);
            }
        }

        internal Dictionary<string, Models.FileMetaInformation> DeserializeFileMetaInformation()
        {
            Dictionary<string, Models.FileMetaInformation> metaInformationFiles = new Dictionary<string, Models.FileMetaInformation>();

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("MetaInformationFiles.dat", FileMode.OpenOrCreate))
            {
                Dictionary<string, Models.FileMetaInformation> newMetaInformationFiles = (Dictionary<string, Models.FileMetaInformation>)formatter.Deserialize(fs);

                metaInformationFiles = newMetaInformationFiles;
            }

            return metaInformationFiles;
        }

        internal void ShowMetaInformation(string fileName) // interface method. Temporarily here
        {
            Dictionary<string, Models.FileMetaInformation> metaInformationFiles = DeserializeFileMetaInformation();

            Models.FileMetaInformation activeFile = metaInformationFiles.GetValueOrDefault(fileName);
            
            Console.WriteLine("File name: " + activeFile.name);
            Console.WriteLine("file extension: " + activeFile.extension);
            Console.WriteLine("File size: " + activeFile.size + " kb");
            Console.WriteLine("Date of upload: " + activeFile.creationDate);
            Console.WriteLine("Number of downloads: " + activeFile.downloadsNumber);
        }
    }
}
