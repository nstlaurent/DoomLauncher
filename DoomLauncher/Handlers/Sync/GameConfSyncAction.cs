using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace DoomLauncher.Handlers.Sync
{
    public class GameConfSyncAction : ISyncAction
    {
        private readonly IDataSourceAdapter m_database;

        public GameConfSyncAction(IDataSourceAdapter database)
        {
            m_database = database;
        }

        public SyncResult ApplyToGameFile(IGameFile gameFile, IArchiveReader reader, string[] mapInfoData)
        {
            var entry = reader.Entries.FirstOrDefault(e => e.Name.ToLower().Equals("gameconf"));
            if (entry != null)
            {
                var json = entry.ReadString(Encoding.UTF7);

                try
                {
                    var serializer = new JavaScriptSerializer();
                    var gameConfData = serializer.Deserialize<Dictionary<string, object>>(json);

                    var data = (Dictionary<string, object>)gameConfData["data"];
                    var title = data.ContainsKey("title") ? data["title"] as string : null;
                    var author = data.ContainsKey("author") ? data["author"] as string : null;
                    var description = data.ContainsKey("description") ? data["description"] as string : null;
                    var iwad = data.ContainsKey("iwad") ? data["iwad"] as string : null;

                    if (!string.IsNullOrEmpty(title))
                        gameFile.Title = title;

                    if (!string.IsNullOrEmpty(author))
                        gameFile.Author = author;

                    if (!string.IsNullOrEmpty(description))
                        gameFile.Description = description;

                    if (!string.IsNullOrEmpty(iwad))
                    {
                        var gameName = Path.GetFileNameWithoutExtension(iwad.ToLower());
                        var iwadFile = m_database.GetIWads().FirstOrDefault(x =>
                            Path.GetFileNameWithoutExtension(x.Name.ToLower()).Equals(gameName));
                        if (iwadFile != null)
                            gameFile.IWadID = iwadFile.IWadID;
                    }
                }
                catch
                {

                }
            }

            return SyncResult.EMPTY;
        }
    }
}
