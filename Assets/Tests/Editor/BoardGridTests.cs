using NUnit.Framework;

// The gear board's cells (BoardGrid, in GearManager.cs) [review candidate 3].
public class BoardGridTests
{
    private const int Hp = 9;
    private const int Attack = 10;

    [Test]
    public void New_AllCellsEmpty()
    {
        BoardGrid grid = new BoardGrid();

        for (int row = 0; row < BoardGrid.Rows; row++)
            for (int column = 0; column < BoardGrid.Columns; column++)
                Assert.AreEqual(0, grid.Get(row, column));
    }

    [Test]
    public void TryPlace_EmptyCell_StoresId()
    {
        BoardGrid grid = new BoardGrid();

        Assert.IsTrue(grid.TryPlace(1, 2, Hp));
        Assert.AreEqual(Hp, grid.Get(1, 2));
    }

    [Test]
    public void TryPlace_OccupiedCell_FailsAndKeepsFirst()
    {
        BoardGrid grid = new BoardGrid();
        grid.TryPlace(1, 2, Hp);

        Assert.IsFalse(grid.TryPlace(1, 2, Attack));
        Assert.AreEqual(Hp, grid.Get(1, 2));
    }

    [Test]
    public void TryPlace_OutsideTheGrid_Fails()
    {
        BoardGrid grid = new BoardGrid();

        Assert.IsFalse(grid.TryPlace(BoardGrid.Rows, 0, Hp));
        Assert.IsFalse(grid.TryPlace(0, -1, Hp));
    }

    [Test]
    public void TryPlace_IdZeroOrLess_Fails()
    {
        BoardGrid grid = new BoardGrid();

        Assert.IsFalse(grid.TryPlace(0, 0, 0));
        Assert.IsFalse(grid.TryPlace(0, 0, -3));
    }

    [Test]
    public void Remove_ReturnsTheIdAndEmptiesTheCell()
    {
        BoardGrid grid = new BoardGrid();
        grid.TryPlace(3, 8, Attack);

        Assert.AreEqual(Attack, grid.Remove(3, 8));
        Assert.AreEqual(0, grid.Get(3, 8));
    }
}
