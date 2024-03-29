﻿using BooruSharp.Booru;
using BooruSharp.Search.Tag;

namespace Sanara_PreloadBooruQuizz
{
    class Program
    {
        static async Task Main()
        {
            // ---------- PARAMETERS

            var booru = new Sakugabooru(); // Booru used for the search
            var targetTag = TagType.Copyright; // What tag we try to find
            var stopAfterFirst = true; // Do we stop searching after finding the tag
            var blacklist = new[] { "western", "web" };

            // ----------

            var alreadyTaken = new List<int>();
            var allData = new Dictionary<string, int>();
            var tagsCache = new Dictionary<string, TagType>();

            while (true)
            {
                var results = await booru.GetRandomPostsAsync(int.MaxValue);

                foreach (var post in results)
                {
                    Console.Clear();
                    Console.WriteLine($"Data loaded: {alreadyTaken.Count}\nAnime loaded: {allData.Keys.Count}\nPlease wait...");

                    if (alreadyTaken.Contains(post.ID))
                        continue;
                    alreadyTaken.Add(post.ID);

                    if (post.Tags.Any(x => blacklist.Contains(x)))
                        continue;

                    foreach (var tag in post.Tags)
                    {
                        TagType type;
                        if (!tagsCache.ContainsKey(tag))
                        {
                            SearchResult tagInfo;
                            try
                            {
                                tagInfo = await booru.GetTagAsync(tag);
                            } catch (Exception)
                            { continue; }
                            tagsCache.Add(tag, tagInfo.Type);
                            type = tagInfo.Type;
                        }
                        else
                            type = tagsCache[tag];

                        if (type == targetTag)
                        {
                            if (!allData.ContainsKey(tag))
                                allData.Add(tag, 1);
                            else
                                allData[tag]++;

                            if (stopAfterFirst)
                                break;
                        }

                        File.WriteAllLines("TagsResult.txt", allData.Select(x => x.Key + " " + x.Value));
                    }
                }
            }
        }
    }
}
