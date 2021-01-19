using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP2077SaveEditor
{
    public class AppearanceConstructor
    {
        public class ValueRepresentation
        {
            public string CreatorField { get; set; }
            public string Name { get; set; }

            //object = CharacterCustomizationAppearances.HashValueEntry or CharacterCustomizationAppearances.ValueEntry
            public List<EntryPair> Entries { get; set; }

            public ValueRepresentation(string _creatorField, string _name, List<EntryPair> _entries)
            {
                CreatorField = _creatorField;
                Name = _name;
                Entries = _entries;
            }

            public ValueRepresentation(string _creatorField, string _name)
            {
                CreatorField = _creatorField;
                Name = _name;
                Entries = new List<EntryPair>();
            }
        }

        public class EntryLocation
        {
            public string SectionName { get; set; }
            public string AppearanceSectionName { get; set; }

            public EntryLocation(string _name, string _appearanceName)
            {
                SectionName = _name;
                AppearanceSectionName = _appearanceName;
            }
        }

        public class EntryPair
        {
            public EntryLocation Location { get; set; }
            public object Entry { get; set; }

            public EntryPair(EntryLocation _loc, object _entry)
            {
                Location = _loc;
                Entry = _entry;
            }
        }
    }
}
