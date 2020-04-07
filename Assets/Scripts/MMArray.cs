using System;
using System.Collections;
using UnityEngine;

public class MMArray : IEnumerator, IEnumerable
{
    unsafe float* arrayPointer;
    public readonly int Length;

    int position = -1;

    public unsafe MMArray(float* arrayPointer, int length)
    {
        this.arrayPointer = arrayPointer;
        this.Length = length;
    }

    //public unsafe MMArray(int length)
    //{
    //    this.arrayPointer = new float*[length];
    //    this.Length = length;
    //}

    public unsafe float this[int index]
    {
        get
        {
            if (index >= this.Length)
                throw new IndexOutOfRangeException();
            return *(arrayPointer + index);
        }

        set
        {
            if (index >= this.Length)
                throw new IndexOutOfRangeException();
            *(arrayPointer + index) = value;
        }
    }

    public void Zero()
    {
        for (int i = 0; i < Length; i++)
            this[i] = 0;
    }

    public IEnumerator GetEnumerator()
    {
        this.position=-1;
        return (IEnumerator)this;
    }

    //IEnumerator
    public bool MoveNext()
    {
        this.position++;
        return (this.position < this.Length);
    }

    //IEnumerable
    public void Reset()
    { this.position = -1; }

    //IEnumerable
    public object Current
    {
        get { return this[this.position]; }
    }
}
