using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameModes
    {
        Random = 1,
        Classic = 2,
        Twist = 3
    }

    public class Shape
    {
        public List<Tuple<int, int>> GetQuadrant(int i)
        {
            if (i == 0)
                return I;
            else if (i == 1)
                return II;
            else if (i == 2)
                return III;
            else if (i == 3)
                return IV;
            else
                return new List<Tuple<int, int>>();
        }

        public int Quadrants { get; set; }
        public List<Tuple<int, int>> I { get; set; }
        public List<Tuple<int, int>> II { get; set; }
        public List<Tuple<int, int>> III { get; set; }
        public List<Tuple<int, int>> IV { get; set; }
    }

    public static class Shapes
    {
        /// <summary>
        /// C# matrices are built with (0,0) in the top left, and (n,n) in the bottom right
        /// So, the shapes will be need to be built based on this and not a standard euclidian x,y grid
        /// </summary>
        /////////////////////////
        // 
        // (-1,-1) ( 0,-1) ( 1,-1)
        // (-1, 0) ( 0,0 ) ( 1,0 )
        // (-1, 1) ( 0,1 ) ( 1,1 )
        //
        //////////////////////////

        public static Dictionary<int, Shape> ShapesList = new Dictionary<int, Shape>()
        {
            { 0,
                new Shape()
                {
                    Quadrants = 1,
                    I = new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(0,0)
                    }
                }
            },
            { 1, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // One block in middle, one above
                    { 
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1)
                    },
                    II = new List<Tuple<int, int>>() // One block in middle, one left
                    { 
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,0) 
                    }
                }
            },
            { 2,
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // One block in middle, one above, one below
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(0,-1)
                    },
                    II =  new List<Tuple<int, int>>() // One block in middle, one left, one right
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0)
                    }
                }
            },
            { 3, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // One block in middle, diagonal right
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,-1)
                    },
                    II = new List<Tuple<int, int>>() // One block in middle, diagonal left
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,-1)
                    }
                }
            },
            { 4, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // One block in middle, bottom right, top left
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,1),
                        new Tuple<int, int>(-1,-1)
                    },
                    II = new List<Tuple<int, int>>() // One block in middle, bottom left, top right
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(1,-1)
                    }
                }
            },
            { 5, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 3 block L bottom left
                    {
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(0,1)
                    },
                    II = new List<Tuple<int, int>>() // 3 block L top left
                    {
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,-1),
                        new Tuple<int, int>(0,-1)
                    },
                    III = new List<Tuple<int, int>>() // 3 block L top right
                    {
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(1,-1),
                        new Tuple<int, int>(1,0)
                    },
                    IV = new List<Tuple<int, int>>() // 3 block L bottom right
                    {
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(1,1),
                        new Tuple<int, int>(0,1)
                    }
                }
            },
            { 6, 
                new Shape()
                {
                    Quadrants = 1,
                    I = new List<Tuple<int, int>>() // 4 block square
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(0,1)
                    }
                }
            },
            { 7, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 4 block 3 blocks up/down, one block right
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,0)
                    },
                    II = new List<Tuple<int, int>>() // 4 block 3 blocks left/right, one block bottom
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(0,1)
                    },
                    III = new List<Tuple<int, int>>() // 4 block 3 blocks up/down, one block left
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(-1,0)
                    },
                    IV = new List<Tuple<int, int>>() // 4 block 3 blocks left/right, one block top
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(0,-1)
                    }
                }
            },
            { 8, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // 4 block S
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(-1,1)
                    },
                    II = new List<Tuple<int, int>>() // 4 block S rotated
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(1,1)
                    }
                }
            },
            { 9, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // 4 block backwards s
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,1)
                    },
                    II = new List<Tuple<int, int>>() // 4 block backwards S rotated
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(1,-1)
                    }
                }
            },
            { 10, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 4 block L
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,1)
                    },
                    II = new List<Tuple<int, int>>() // 4 block L rotated 90
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,1)
                    },
                    III = new List<Tuple<int, int>>() // 4 block L rotated 180
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(-1,-1)
                    },
                    IV = new List<Tuple<int, int>>() // 4 block L rotated 270
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,-1)
                    }
                }
            },
            { 11, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 4 block mirror L
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(-1,1)
                    },
                    II = new List<Tuple<int, int>>() // 4 block mirror L rotated 90
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,-1)
                    },
                    III = new List<Tuple<int, int>>() // 4 block mirror L rotated 180
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,-1)
                    },
                    IV = new List<Tuple<int, int>>() // 4 block mirror L rotated 270
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,1)
                    }
                }
            },
            { 12, 
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // 4 block line horizontal
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(2,0),
                        new Tuple<int, int>(-1,0)
                    },
                    II = new List<Tuple<int, int>>() // 4 block line vertical
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,-2),
                        new Tuple<int, int>(0,1)
                    }
                }
            },
            { 13,
                new Shape()
                {
                    Quadrants = 2,
                    I = new List<Tuple<int, int>>() // 5 block line horizontal
                    { 
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(2,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-2,0) 
                    },
                    II = new List<Tuple<int, int>>() // 5 block line vertical
                    { 
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,-2),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(0,2) 
                    }
                }
            },
            { 14, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 5 block T
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(-1,-1),
                        new Tuple<int, int>(1,-1),
                        new Tuple<int, int>(0,1)
                    },
                    II = new List<Tuple<int, int>>() // 5 block T facing right
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(1,1),
                        new Tuple<int, int>(1,-1)
                    },
                    III = new List<Tuple<int, int>>() // 5 block T upside down
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(1,1),
                        new Tuple<int, int>(0,-1)
                    },
                    IV = new List<Tuple<int, int>>() // 5 block T facing Left
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(-1,-1)
                    }
                }
            },
            { 15, 
                new Shape()
                {
                    Quadrants = 4,
                    I = new List<Tuple<int, int>>() // 5 block U
                    {
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(0,1),
                        new Tuple<int, int>(1,1)
                    },
                    II = new List<Tuple<int, int>>() // 5 block U facing right
                    {
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(-1,-1),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(-1,1),
                        new Tuple<int, int>(0,1)
                    },
                    III = new List<Tuple<int, int>>() // 5 block U upsideown
                    {
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,-1),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(1,-1)
                    },
                    IV = new List<Tuple<int, int>>() // 5 block U facing left
                    {
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(1,-1),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(1,1),
                        new Tuple<int, int>(0,1)
                    }
                }
            },
            { 16, 
                new Shape()
                {
                    Quadrants = 1,
                    I = new List<Tuple<int, int>>() // 5 block +
                    {
                        new Tuple<int, int>(0,0),
                        new Tuple<int, int>(1,0),
                        new Tuple<int, int>(-1,0),
                        new Tuple<int, int>(0,-1),
                        new Tuple<int, int>(0,1)
                    }
                }
            }
        };
    }
}
