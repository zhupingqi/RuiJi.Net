using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Core.Extracter
{
    public class ExtractResult
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public int Group { get; set; }

        public List<ExtractResult> Tiles { get; set; }

        public Dictionary<int, List<ExtractResult>> TileGroup
        {
            get
            {
                if (Tiles == null)
                    return null;

                return Tiles.GroupBy(m => m.Group).ToDictionary(m => m.Key, m => m.ToList());
            }
        }
    }

    public class ExtractResultCollection
    {
        public List<ExtractResult> Results
        {
            get;
            private set;
        }

        public ExtractResultCollection()
        {
            Results = new List<ExtractResult>();
        }

        public string[] AllKeys
        {
            get
            {
                return Results.Select(m => m.Name).Distinct().ToArray();
            }
        }

        public List<ExtractResult> this[string name]
        {
            get
            {
                return Results.Where(m => m.Name == name).ToList();
            }
        }

        public ExtractResult this[int index]
        {
            get
            {
                return Results[index];
            }
        }

        public void Add(ExtractResult result)
        {
            Results.Add(result);
        }

        public void Add(ExtractResultCollection collection)
        {
            foreach (var r in collection.Results)
            {
                Add(r);
            }
        }

        public void Clear()
        {
            Results.Clear();
        }

        public bool HasKeys()
        {
            return AllKeys.Length > 0;
        }
    }
}