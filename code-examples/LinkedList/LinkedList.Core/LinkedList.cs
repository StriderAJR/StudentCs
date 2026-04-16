using System.Collections;

namespace LinkedList;

public class LinkedList<TElement> : IEnumerable<TElement>
{
    private class Node
    {
        public TElement Value;
        public Node Next;
    }

    private Node head;
    private Node tail;
    private int count;

    public int Count => count;

    public void Add(TElement value)
    {
        Node newNode = new Node();
        newNode.Value = value;

        if (head == null)
        {
            head = newNode;
        }
        else
        {
            tail.Next = newNode;
            tail = newNode;
        }

        count++;
    }

    public bool Add(TElement value, int index)
    {
        if (index < 0 || index > count)
            return false;

        Node newNode = new Node();
        newNode.Value = value;

        if (index == 0)
        {
            newNode.Next = head;
            head = newNode;
        }
        else
        {
            Node prev = GetNode(index - 1);
            newNode.Next = prev.Next;
            prev.Next = newNode;
        }

        count++;
        return true;
    }

    public void RemoveByIndex(int index)
    {
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException();

        if (index == 0)
        {
            head = head.Next;
        }
        else
        {
            Node prev = GetNode(index - 1);
            prev.Next = prev.Next.Next;
        }

        count--;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Sort(Func<TElement, TElement, int> comparator)
    {
        // check for empty or single element
        if (head == null || head.Next == null)
        {
            return;
        }

        bool swapped;
        do
        {
            swapped = false;
            Node current = head;

            while (current.Next != null)
            {
                // compare current and next
                if (comparator(current.Value, current.Next.Value) > 0)
                {
                    // swap values
                    TElement temp = current.Value;
                    current.Value = current.Next.Value;
                    current.Next.Value = temp;

                    swapped = true;
                }

                current = current.Next;
            }
        }
        while (swapped);
    }

    public IEnumerator<TElement> GetEnumerator()
    {
        Node current = head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    public TElement this[int index]
    {
        get
        {
            return GetByIndex(index);
        }
        set
        {
            SetByIndex(index, value);
        }
    }

    private TElement GetByIndex(int index)
    {
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException();

        return GetNode(index).Value;
    }

    private void SetByIndex(int index, TElement value)
    {
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException();

        GetNode(index).Value = value;
    }

    private Node GetNode(int index)
    {
        Node current = head;

        for (int i = 0; i < index; i++)
            current = current.Next;

        return current;
    }
}

public static class LinkedListExtensions
{
    public static IEnumerable<T> MyWhere<T>(this LinkedList<T> source, Func<T, bool> predicate)
    {
        // validate arguments
        if (source == null)
        {
            throw new ArgumentNullException();
        }

        if (predicate == null)
        {
            throw new ArgumentNullException();
        }

        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }
}
