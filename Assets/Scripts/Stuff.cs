using System;
using System.Collections;
using UnityEngine;

namespace UPC
{
    public class MMArray : IEnumerator, IEnumerable
    {
        unsafe float* arrayPointer;
        public readonly int length;

        int position = -1;

        public unsafe MMArray(float* arrayPointer, int length)
        {
            this.arrayPointer = arrayPointer;
            this.length = length;
        }

        public unsafe float this[int index]
        {
            get
            {
                if (index >= this.length)
                    throw new IndexOutOfRangeException();
                return *(arrayPointer + index);
            }

            set
            {
                if (index >= this.length)
                    throw new IndexOutOfRangeException();
                *(arrayPointer + index) = value;
            }
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
            return (this.position < this.length);
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



}
