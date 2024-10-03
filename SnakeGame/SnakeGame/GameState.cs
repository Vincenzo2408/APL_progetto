using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameState
    {
        //properties
        public int Rows { get; }
        public int Cols { get; }
        public Griglia[,] Grid { get; }
        public Direzioni Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        //correggiamo movimento
        private readonly LinkedList<Direzioni> dirChanges = new LinkedList<Direzioni>();
        //lista delle posizioni occupate dal serpente, usiamo la LinkedList perchè ci permette di aggiugere e rimuovere da entrambi inizio/fine della lista
        private readonly LinkedList<Posizioni> snakePositions = new LinkedList<Posizioni>();
        //per cibo
        private readonly Random random = new Random();

        //definiamo costruttore
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new Griglia[rows, cols];
            //quando il gioco inizia, settiamo la direzione di partenza a destra
            Dir = Direzioni.Right;

            AddSnake();
            AddFood();
        }

        //aggiungiamo serpente alla griglia
        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = Griglia.Snake;
                snakePositions.AddFirst(new Posizioni(r, c));
            }
        }

        //metodo che ritorna tutte le posizioni vuote della griglia
        private IEnumerable<Posizioni> EmptyPositions()
        {
            for (int r=0; r < Rows; r++)
            {
                for (int c=0; c<Cols; c++)
                {
                    if (Grid[r, c] == Griglia.Empty)
                    {
                        yield return new Posizioni(r, c);
                    }
                }
            }
        }

        //aggiungiamo serpente alla griglia
        private void AddFood()
        {
            List<Posizioni> empty = new List<Posizioni>(EmptyPositions());
            
            if (empty.Count == 0)
            {
                return; //vittoria
            }

            Posizioni pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = Griglia.Food;
        }

        public Posizioni HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Posizioni TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Posizioni> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Posizioni pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = Griglia.Snake;
        }

        private void RemoveTail()
        {
            Posizioni tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = Griglia.Empty;
            snakePositions.RemoveLast();
        }

        private Direzioni GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direzioni newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direzioni lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDirection(Direzioni dir)
        {
            //Dir = dir;

            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        private bool OutsideGrid(Posizioni pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private Griglia WillHit(Posizioni newHeadPos)
        {
            //gestione caso in cui il serpente esce fuori dalla griglia
            if (OutsideGrid(newHeadPos))
            {
                return Griglia.Outside;
            }

            //gestione caso particolare in cui serpente si quasi morde la coda
            if (newHeadPos == TailPosition())
            {
                return Griglia.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Posizioni newHeadPos = HeadPosition().Translate(Dir);
            Griglia hit = WillHit(newHeadPos);

            if(hit == Griglia.Outside || hit == Griglia.Snake)
            {
                GameOver = true;
            }
            else if (hit==Griglia.Empty) 
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == Griglia.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
