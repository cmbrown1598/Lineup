namespace Algorithms
{
    public class PlayerItem
    {
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlayerItem);
        }

        protected bool Equals(PlayerItem other)
        {
            return other != null && string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}