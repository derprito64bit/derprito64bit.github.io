using System;

namespace Ion.Portfolio
{
    /// <summary>
    /// Points an exhibit at its content entry: <see cref="Kind"/> names the file (project, honour, workshop, study)
    /// and <see cref="Id"/> the entry's slug or id. Serializable, so a photo copy of an exhibit keeps it.
    /// </summary>
    [Serializable]
    public struct ExhibitRef
    {
        public string Kind;
        public string Id;

        public ExhibitRef(string kind, string id)
        {
            Kind = kind;
            Id = id;
        }

        public static ExhibitRef Project(string slug) => new ExhibitRef("project", slug);
        public static ExhibitRef Honour(string id) => new ExhibitRef("honour", id);
        public static ExhibitRef Workshop(string id) => new ExhibitRef("workshop", id);
        public static ExhibitRef Study(string id) => new ExhibitRef("study", id);

        /// <summary>An empty slot: nothing to read.</summary>
        public bool IsEmpty => string.IsNullOrEmpty(Kind) || string.IsNullOrEmpty(Id);

        public override string ToString() => IsEmpty ? "(empty)" : Kind + ":" + Id;
    }
}
