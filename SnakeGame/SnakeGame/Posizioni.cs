using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public class Posizioni
    {
        public int Row { get; }
        public int Col { get; }

        //definiamo costruttore
        public Posizioni(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Posizioni Translate(Direzioni dir)
        {
            return new Posizioni(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        //override di equals e gethash (generati automaticamente) per potere utilizzare la classe Posizioni come chiave
        public override bool Equals(object obj)
        {
            return obj is Posizioni posizioni &&
                   Row == posizioni.Row &&
                   Col == posizioni.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Posizioni left, Posizioni right)
        {
            return EqualityComparer<Posizioni>.Default.Equals(left, right);
        }

        public static bool operator !=(Posizioni left, Posizioni right)
        {
            return !(left == right);
        }
    }
}
