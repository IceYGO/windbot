using System.Data;

namespace YGOSharp.OCGWrapper
{
    public class NamedCard : Card
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        internal NamedCard(IDataRecord reader) : base(reader)
        {
            Name = reader.GetString(10);
            Description = reader.GetString(11);
        }

        public static new NamedCard Get(int id)
        {
            return NamedCardsManager.GetCard(id);
        }
    }
}
