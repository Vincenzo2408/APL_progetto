using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class Direzioni
    {
        public readonly static Direzioni Left = new Direzioni(0, -1);
        public readonly static Direzioni Right = new Direzioni(0, 1);
        public readonly static Direzioni Up = new Direzioni(-1, 0);
        public readonly static Direzioni Down = new Direzioni(1, 0);

        public int RowOffset { get; }
        public int ColOffset { get; }

        //definiamo costruttore
        private Direzioni(int rowOffset, int colOffset) 
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        public Direzioni Opposite()
        {
            return new Direzioni(-RowOffset, -ColOffset);
        }

        //override di equals e gethash (generati automaticamente) per potere utilizzare la classe Direzioni come chiave
        public override bool Equals(object obj)
        {
            return obj is Direzioni direzioni &&
                   RowOffset == direzioni.RowOffset &&
                   ColOffset == direzioni.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Direzioni left, Direzioni right)
        {
            return EqualityComparer<Direzioni>.Default.Equals(left, right);
        }

        public static bool operator !=(Direzioni left, Direzioni right)
        {
            return !(left == right);
        }
    }
}
