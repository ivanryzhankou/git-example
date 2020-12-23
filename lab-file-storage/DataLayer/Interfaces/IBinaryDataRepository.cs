using System.Collections.Generic;

namespace DataLayer.Interfaces
{
    public interface IBinaryDataRepository
    {
        void SerializeFileMetaInformation(Dictionary<string, Models.FileMetaInformation> metaInformationFiles);
        Dictionary<string, Models.FileMetaInformation> DeserializeFileMetaInformation();
    }
}
