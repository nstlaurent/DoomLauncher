using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoomLauncher
{
    public class TagMapLookup : ITagMapLookup
    {
        public event EventHandler<ITagData[]> TagMappingChanged;

        private readonly IDataSourceAdapter m_adapter;

        private Dictionary<int, ITagMapping[]> m_fileTagMapping;
        private Dictionary<int, ITagData> m_tags;

        public TagMapLookup(IDataSourceAdapter adapter)
        {
            m_adapter = adapter;

            Refresh(new ITagData[] { });
        }

        private Dictionary<int, ITagMapping[]> BuildTagMappingDictionary(IEnumerable<ITagMapping> tagMapping)
        {
            Dictionary<int, ITagMapping[]> ret = new Dictionary<int, ITagMapping[]>();

            int[] files = tagMapping.Select(x => x.FileID).Distinct().ToArray();
            Array.ForEach(files, x => ret.Add(x, tagMapping.Where(y => y.FileID == x).ToArray()));

            return ret;
        }

        public void Refresh(ITagData[] tags)
        {
            IEnumerable<ITagMapping> tagMapping = m_adapter.GetTagMappings().OrderBy(x => x.FileID);
            m_fileTagMapping = BuildTagMappingDictionary(tagMapping);

            m_tags = m_adapter.GetTags().ToDictionary(x => x.TagID, x => x);

            TagMappingChanged?.Invoke(this, tags);
        }

        public ITagData[] GetTags(IGameFile gameFile)
        {
            if (gameFile != null && gameFile.GameFileID.HasValue && m_fileTagMapping.ContainsKey(gameFile.GameFileID.Value))
            {
                ITagMapping[] tagMapping = m_fileTagMapping[gameFile.GameFileID.Value];

                return tagMapping.Where(k => m_tags.ContainsKey(k.TagID))
                     .Select(k => m_tags[k.TagID]).ToArray();
            }

            return new ITagData[] { };
        }
    }
}
