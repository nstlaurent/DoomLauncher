using System.Collections.Generic;

namespace DoomLauncher.Interfaces
{
    public interface IConfigurationDataSourceAdapter
    {
        IEnumerable<IConfigurationData> GetConfiguration();
        void InsertConfiguration(IConfigurationData config);
        void UpdateConfiguration(IConfigurationData config);

        IEnumerable<ITagData> GetTags();
        void InsertTag(ITagData tag);
        void UpdateTag(ITagData tag);
        void DeleteTag(ITagData tag);
        IEnumerable<ITagMapping> GetTagMappings();
        IEnumerable<ITagMapping> GetTagMappings(int gameFileID);
        void InsertTagMapping(ITagMapping tag);
        void DeleteTagMapping(ITagMapping tag);
        void DeleteTagMapping(int tagID);
    }
}
