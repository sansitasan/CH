using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    public struct PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _heap;

        public PriorityQueue(int count = 1)
        {
            _heap = new List<T>(count);
        }

        public void Push(T value)
        {
            _heap.Add(value);

            int now = _heap.Count - 1;
            while (now > 0)
            {
                int next = (now - 1) >> 1;

                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break;

                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                now = next;
            }
        }

        public T Pop()
        {
            if (_heap.Count == 0)
            {
                Debug.Assert(_heap.Count == 0, "Heap Count is Zero");
            }
            T ret = _heap[0];
            T temp;

            int left, right, next;
            int lastidx = _heap.Count - 1;
            _heap[0] = _heap[lastidx];
            _heap.RemoveAt(lastidx);
            --lastidx;

            int now = 0;

            while (true)
            {
                left = 1 + (now >> 1);
                right = (now + 1) >> 1;

                next = now;
                if (left <= lastidx && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;

                if (right <= lastidx && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                if (next == now)
                    break;

                temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                now = next;
            }

            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }


    public class AStar
    {
        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };
        private struct PQNode : IComparable<PQNode>
        {
            public readonly int F;
            public readonly int G;
            public readonly int X;
            public readonly int Y;

            public PQNode(int f = 0, int g = 0, int x = 0, int y = 0)
            {
                F = f;
                G = g;
                X = x; 
                Y = y;
            }

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
            }
        }

        public void Astar(int xSize, int ySize, int startX, int startY, int destX, int destY)
        {
            //점수 매기기
            //F = G + H
            //F가 최종 점수
            //G가 시작점에서 목적지까지 도달하는데 드는 비용
            //H가 목적지에서 얼마나 가까운지(휴리스틱, 고정)
            //visit 여부를 넣어두자
            PQNode node;
            bool[,] closed = new bool[xSize, ySize];
            int nextX, nextY;
            //가는 길을 발견했는가
            //발견하지 못했으면 -1 or 맥스벨류,
            //발견했으면 F를 넣자
            int[,] open = new int[xSize, ySize];
            for (int y = 0; y < ySize; ++y)
            {
                for (int x = 0; x < xSize; ++x)
                {
                    open[y, x] = Int32.MaxValue;
                }
            }

            open[startY, startX] = Math.Abs(destY - startY) + Math.Abs(destX - startX);

            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
            

            pq.Push(new PQNode(
                f: Math.Abs(destY - startY) + Math.Abs(destX - startX),
                g: 0,
                x: startX,
                y: startY
                ));

            while (true)
            {
                int t = pq.Count();
                if (t == 0)
                    break;
                node = pq.Pop();
                if (closed[node.Y, node.X])
                    continue;

                closed[node.Y, node.X] = true;
                if (node.Y == destY && node.X == destX)
                    break;

                for (int i = 0; i < 4; ++i)
                {
                    nextX = node.X + deltaX[i];
                    nextY = node.Y + deltaY[i];

                    if (nextX < 0 || nextX > xSize || nextY < 0 || nextY > ySize)
                        continue;
                    //벽도 아웃
                    if (closed[nextY, nextX])
                        continue;

                    //cost를 더하자
                    int g = node.G + 1;

                    int h = Math.Abs(destY - nextY) + Math.Abs(destX - nextX);
                    if (open[nextY, nextX] < g + h)
                        continue;

                    open[nextY, nextX] = g + h;
                    pq.Push(new PQNode(
                        f: g + h,
                        g: g,
                        y: nextY,
                        x: nextX
                        ));

                    
                }
            }
        }
    }
}