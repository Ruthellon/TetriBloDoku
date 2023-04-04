using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Shapes
    {
        public static Dictionary<int, List<Tuple<int, int>>> ShapesList = new Dictionary<int, List<Tuple<int, int>>>()
        {
            { 0, new List<Tuple<int, int>>() { // One block in middle
                new Tuple<int, int>(0,0) }
            },
            { 1, new List<Tuple<int, int>>() { // One block in middle, one above
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1) }
            },
            { 2, new List<Tuple<int, int>>() { // One block in middle, one above, one below
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1) }
            },
            { 3, new List<Tuple<int, int>>() { // One block in middle, one left
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(-1,0) }
            },
            { 4, new List<Tuple<int, int>>() { // One block in middle, one left, one right
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0) }
            },
            { 5, new List<Tuple<int, int>>() { // One block in middle, diagonal right
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,1) }
            },
            { 6, new List<Tuple<int, int>>() { // One block in middle, one diagnol right, one diagonal left
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,1),
                new Tuple<int, int>(-1,-1) }
            },
            { 7, new List<Tuple<int, int>>() { // One block in middle, diagonal left
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(-1,1) }
            },
            { 8, new List<Tuple<int, int>>() { // One block in middle, one diagnol right, one diagonal left
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(-1,1),
                new Tuple<int, int>(1,-1) }
            },
            { 9, new List<Tuple<int, int>>() { // 3 block L bottom left
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(-1,-1),
                new Tuple<int, int>(0,-1) }
            },
            { 10, new List<Tuple<int, int>>() { // 3 block L top left
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(-1,1),
                new Tuple<int, int>(0,1) }
            },
            { 11, new List<Tuple<int, int>>() { // 3 block L top right
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,1),
                new Tuple<int, int>(1,0) }
            },
            { 12, new List<Tuple<int, int>>() { // 3 block L bottom right
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(1,-1),
                new Tuple<int, int>(0,-1) }
            },
            { 13, new List<Tuple<int, int>>() { // 4 block square
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(-1,-1),
                new Tuple<int, int>(0,-1) }
            },
            { 14, new List<Tuple<int, int>>() { // 4 block 3 blocks up/down, one block right
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,0) }
            },
            { 15, new List<Tuple<int, int>>() { // 4 block 3 blocks up/down, one block left
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(-1,0) }
            },
            { 16, new List<Tuple<int, int>>() { // 4 block 3 blocks left/right, one block top
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(0,1) }
            },
            { 17, new List<Tuple<int, int>>() { // 4 block 3 blocks left/right, one block bottom
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(0,-1) }
            },
            { 18, new List<Tuple<int, int>>() { // 4 block S
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(-1,-1) }
            },
            { 19, new List<Tuple<int, int>>() { // 4 block S rotated
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(1,-1) }
            },
            { 20, new List<Tuple<int, int>>() { // 4 block backwards s
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,-1) }
            },
            { 21, new List<Tuple<int, int>>() { // 4 block backwards S rotated
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(1,1) }
            },
            { 22, new List<Tuple<int, int>>() { // 4 block L
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,-1) }
            },
            { 23, new List<Tuple<int, int>>() { // 4 block L rotated 90
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(-1,-1) }
            },
            { 24, new List<Tuple<int, int>>() { // 4 block L rotated 180
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(-1,1) }
            },
            { 25, new List<Tuple<int, int>>() { // 4 block L rotated 270
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(1,1) }
            },
            { 26, new List<Tuple<int, int>>() { // 4 block mirror L
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(-1,-1) }
            },
            { 27, new List<Tuple<int, int>>() { // 4 block mirror L rotated 90
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(-1,1) }
            },
            { 28, new List<Tuple<int, int>>() { // 4 block mirror L rotated 180
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(0,-1),
                new Tuple<int, int>(1,1) }
            },
            { 29, new List<Tuple<int, int>>() { // 4 block mirror L rotated 270
                new Tuple<int, int>(0,0),
                new Tuple<int, int>(1,0),
                new Tuple<int, int>(-1,0),
                new Tuple<int, int>(1,-1) }
            }
        };
    }
}
