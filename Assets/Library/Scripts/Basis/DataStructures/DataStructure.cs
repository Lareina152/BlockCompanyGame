using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Basis {

    public class Ellipsoid {
        public Vector3 pivot;
        public Vector3 radius;
        public Vector3 radiusSquare;

        public Ellipsoid(Vector3 pivot, Vector3 radius) {
            this.pivot = pivot;
            this.radius = radius;

            radiusSquare = radius.Pow(2);
        }

        public bool Contains(Vector3 pos) {
            //note.Log($"pos:{pos}, pivot:{pivot}, radius:{radius}");

            Vector3 relative = pos - pivot;

            if (relative.AnyNumberAbove(radius))
            {
                return false;
            }

            return relative.x * relative.x / radiusSquare.x + relative.y * relative.y / radiusSquare.y + relative.z * relative.z / radiusSquare.z <= 1;
        }
    }

    public enum HeapType
    {
        MinHeap,
        MaxHeap
    }

    public class BinaryHeap<T> where T : IComparable<T>
    {
        readonly List<T> items;

        public HeapType HType { get; private set; }
        public int Count => items.Count;

        public T Root => items[0];

        public BinaryHeap(HeapType type) {
            items = new List<T>();
            this.HType = type;
        }

        public void Push(T item) {
            items.Add(item);

            int i = items.Count - 1;

            bool flag = HType == HeapType.MinHeap;

            while (i > 0)
            {
                if ((items[i].CompareTo(items[(i - 1) / 2]) > 0) ^ flag)
                {
                    (items[i], items[(i - 1) / 2]) = (items[(i - 1) / 2], items[i]);
                    i = (i - 1) / 2;
                }
                else
                    break;
            }
        }

        private void DeleteRoot() {
            int i = items.Count - 1;

            items[0] = items[i];
            items.RemoveAt(i);

            i = 0;

            bool flag = HType == HeapType.MinHeap;

            while (true)
            {
                int leftInd = 2 * i + 1;
                int rightInd = 2 * i + 2;
                int largest = i;

                if (leftInd < items.Count)
                {
                    if ((items[leftInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = leftInd;
                }

                if (rightInd < items.Count)
                {
                    if ((items[rightInd].CompareTo(items[largest]) > 0) ^ flag)
                        largest = rightInd;
                }

                if (largest != i)
                {
                    (items[largest], items[i]) = (items[i], items[largest]);
                    i = largest;
                }
                else
                    break;
            }
        }

        public T PopRoot() {
            T result = items[0];

            DeleteRoot();

            return result;
        }

        public T GetRoot() {
            return items[0];
        }

        public override string ToString() {
            return items.ToString(",");
        }
    }
}

