using System.Collections.Generic;
using NUnit.Framework;

// ChainWalk finds the gear chain a producer belongs to; stat gears buff exactly that chain.
public class ChainWalkTests
{
    private class Node
    {
        public readonly string name;
        public readonly List<Node> neighbors = new List<Node>();
        public bool alive = true;

        public Node(string name)
        {
            this.name = name;
        }

        public static void Link(Node a, Node b)
        {
            a.neighbors.Add(b);
            b.neighbors.Add(a);
        }
    }

    private static List<Node> Walk(Node start)
    {
        List<Node> found = new List<Node>();
        ChainWalk.Collect(start, n => n.neighbors, n => n.alive, found);
        return found;
    }

    [Test]
    public void LoneNode_ReturnsOnlyItself()
    {
        Node a = new Node("a");

        CollectionAssert.AreEqual(new[] { a }, Walk(a));
    }

    [Test]
    public void Line_ReturnsEveryNodeFromAnyEnd()
    {
        Node a = new Node("a"), b = new Node("b"), c = new Node("c");
        Node.Link(a, b);
        Node.Link(b, c);

        CollectionAssert.AreEquivalent(new[] { a, b, c }, Walk(a));
        CollectionAssert.AreEquivalent(new[] { a, b, c }, Walk(c));
    }

    [Test]
    public void Cycle_VisitsEachNodeOnce()
    {
        Node a = new Node("a"), b = new Node("b"), c = new Node("c");
        Node.Link(a, b);
        Node.Link(b, c);
        Node.Link(c, a);

        Assert.AreEqual(3, Walk(a).Count);
    }

    [Test]
    public void SeparateGroup_IsNotReturned()
    {
        Node a = new Node("a"), b = new Node("b"), far = new Node("far"), farther = new Node("farther");
        Node.Link(a, b);
        Node.Link(far, farther);

        CollectionAssert.AreEquivalent(new[] { a, b }, Walk(a));
    }

    [Test]
    public void DestroyedNode_IsSkippedAndNotWalkedThrough()
    {
        Node a = new Node("a"), b = new Node("b"), c = new Node("c");
        Node.Link(a, b);
        Node.Link(b, c);
        b.alive = false;

        CollectionAssert.AreEqual(new[] { a }, Walk(a));
    }

    [Test]
    public void DestroyedStart_ReturnsNothing()
    {
        Node a = new Node("a");
        a.alive = false;

        Assert.AreEqual(0, Walk(a).Count);
    }

    [Test]
    public void NullStart_ReturnsNothing()
    {
        Assert.AreEqual(0, Walk(null).Count);
    }

    [Test]
    public void ReusedList_IsClearedFirst()
    {
        Node a = new Node("a"), b = new Node("b");
        List<Node> found = new List<Node> { b, b, b };

        ChainWalk.Collect(a, n => n.neighbors, n => n.alive, found);

        CollectionAssert.AreEqual(new[] { a }, found);
    }
}
