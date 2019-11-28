using System;
using System.Linq;

namespace tic_tac_toe
{
  enum Square { E, X, O };
  enum Status { Playing, Victory, Defeat, Draw }

  class State
  {
    public bool playerTurn = true;
    public Status status = Status.Playing;
    public Square[] board = Enumerable.Range(0, 9)
      .Select(x => Square.E).ToArray();
  }

  class Program
  {
    static State state = new State();

    static void Main(string[] args)
    {
      while (state.status == Status.Playing)
      {
        Console.Clear();
        Console.WriteLine(View(state));

        var input = Console.ReadKey().KeyChar;
        if (Char.IsDigit(input) && input != '0')
        {
          if (state.playerTurn)
            state = Update(0,
              (int)Char.GetNumericValue(input) - 1, state);

          if (!state.playerTurn && state.status == Status.Playing)
            state = Update(1, AiInput(state.board), state);
        }
      }

      Console.Clear();
      Console.WriteLine(View(state));
    }

    static State Update(int player, int pos, State state)
    {
      if (state.board[pos] != Square.E)
        return state;

      if (TurnWillWin(pos, state.board, state.playerTurn))
      {
        state.status = player == 0 ? Status.Victory : Status.Defeat;
      }
      else if (TurnWillDraw(pos, state.board, state.playerTurn))
      {
        state.status = Status.Draw;
      }

      switch (player)
      {
        case 0:
          state.board[pos] = Square.X;
          state.playerTurn = false;
          return state;
        case 1:
          state.board[pos] = Square.O;
          state.playerTurn = true;
          return state;
        default:
          return state;
      }
    }

    static string BoardToString(Square[] board)
    {
      var i = 0;
      return board.Aggregate("", (acc, next) =>
      {
        var id = next == Square.E ? "." : next.ToString();
        var suffix = ++i % 3 == 0 ? "\n" : "";

        return acc + " " + id + " " + suffix;
      });

    }

    static string View(State state)
    {
      switch (state.status)
      {
        case Status.Playing:
          return ViewBoard(state.board);
        case Status.Victory:
          return ViewVictory(state.board);
        case Status.Defeat:
          return ViewDefeat(state.board);
        case Status.Draw:
          return ViewDraw(state.board);
        default:
          return "No Game Running";
      }
    }

    static string ViewBoard(Square[] board)
    {
      return "Please enter a number from 1-9\n" + BoardToString(board);
    }

    static string ViewVictory(Square[] board)
    {
      return "Victory!\n" + BoardToString(board);
    }

    static string ViewDefeat(Square[] board)
    {
      return "Defeat!\n" + BoardToString(board);
    }

    static string ViewDraw(Square[] board)
    {
      return "Draw!\n" + BoardToString(board);
    }

    static int AiInput(Square[] board)
    {
      var playerMoves = board.Select((x, i) => new { x, i })
          .Where(x => x.x == Square.X).Select(x => x.i);

      var sequence = winners.Where(x =>
        x.Intersect(playerMoves).Count() == 2).FirstOrDefault(x =>
          x.Any(x => board[x] == Square.E));

	  if (board[4] == Square.E)
	  {
		return 4;
	  }
      else if (sequence == null || sequence.Length != 3)
      {
		var playerCorners = playerMoves.Where(x => IsCorner(x));
		var clearCorners = board.Select((x, i) => new { x, i })
		  .Where(x => x.x == Square.E && IsCorner(x.i));

		if (clearCorners.Any())
		{
		  var random = new Random();
		  return clearCorners.OrderBy(x => random.Next()).First().i;
		}
		else
		{
          var moves = board.Select((x, i) => (new { x, i }))
            .Where(x => x.x == Square.E).ToArray();

          var index = new Random().Next(0, moves.Length);
          return moves[index].i;
		}
      }
      else
      {
        var move = sequence.Except(playerMoves).First();
        return move;
      }
    }

    static bool TurnWillWin(int pos, Square[] board, bool playerTurn)
    {
      var glyph = playerTurn ? Square.X : Square.O;
      board[pos] = glyph;

      var moves = board.Select((x, i) => new { x, i })
        .Where(x => x.x == glyph).Select(x => x.i);

      if (moves.Count() <= 2)
        return false;

      if (winners.Any(x => x.Intersect(moves).Count() == 3))
        return true;

      return false;
    }

    static int[][] winners =
        new int[][] {
          new int[] { 0, 1, 2 }, new int[] { 0, 3, 6 },
          new int[] { 3, 4, 5 }, new int[] { 1, 4, 7 },
          new int[] { 6, 7, 8 }, new int[] { 2, 5, 8 },
          new int[] { 0, 4, 8 }, new int[] { 2, 4, 6 }
        };

	static int[] corners = { 0, 2, 6, 8 };

    static bool TurnWillDraw(int pos, Square[] board, bool playerTurn)
    {
      var glyph = playerTurn ? Square.X : Square.O;
      board[pos] = glyph;

      return board.Where(x => x == Square.E).Count() == 0;
    }

	static bool IsCorner(int pos)
	{
      return corners.Contains(pos);
	}
  }
}
