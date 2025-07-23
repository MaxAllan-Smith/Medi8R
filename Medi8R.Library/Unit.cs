namespace Medi8R.Library
{
    public readonly struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Value = new();

        public override bool Equals(object? obj) => obj is Unit;
        public bool Equals(Unit other) => true;
        public override int GetHashCode() => 0;
        public override string ToString() => "()";
    }
}