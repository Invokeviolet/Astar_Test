using Astar_Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Astar_Test
{

    public class ANode
    {
        public bool IsBlock { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        // G score 는 출발점부터 현재 Node 까지 오는 동안 소요되는 비용 / 이동할때마다 계산해줘야 하는 값
        //public int Gscore = 0;
        public int G { get; set; }
        // H score 는 현재 Node 에서 도착점까지 예상되는 비용 (휴리스틱, Heuristic) / 도착지를 정한 순간부터 고정되는 값
        //public int Hscore = 0;
        public int H { get; set; }
        // F score = G score + H score
        //public int Fscore;// = Gscore + Hscore
        public int F { get { return G + H; } }
        // 부모 Node : 현재 Node 로 올 때, 바로 전에 거쳐온 Node
        public ANode Parent = null;

        ANode head = null;
        ANode next = null;
        ANode tail = null;

    }

    internal class Program
    {

        static void Main(string[] args)
        {
            AstarMain am = new AstarMain();
            am.Run();
        }
    }


    internal class AstarMain
    {
        // 프로그램 시작 함수
        public void Run()
        {
            bool[,] cells =
            {
                { false, false, false, false, false, false},
                { false, false, false, false, false, false},
                { false, false, false, false, false, false},
                { false, false, false, false, false, false}
            };

            // 맵 데이터 만들기
            for (int y = 0; y < map_height; y++)
            {
                for (int x = 0; x < map_width; x++)
                {
                    myMaps[y, x] = new ANode() { X= x, Y =y,IsBlock = cells[y, x] };
                }
            }

            findedPath.Clear();
            // 경로 찾기
            FindPath(2, 4, 1, 1); // 출발 지점 좌표, 도착 지점 좌표

            // 찾은 경로 표시하기
            // 반복문을 통해 도착점의 부모노드를 따라서 시작점까지 이동하면서 Node를 기록하고
            // 경로를 출력한다.
            if (findedPath.Count > 0) 
            {
                foreach (ANode node in findedPath)
                {
                    Console.WriteLine($" [{node.Y}, {node.X}]");
                }
            }

        }

        // 맵 데이터
        const int map_height = 4;
        const int map_width = 6;
        ANode[,] myMaps = new ANode[map_height, map_width];

        // Open List : 앞으로 가게 될 노드의 목록
        List<ANode> OpenList = new List<ANode>();
        // Closed List : 이미 방문한 노드의 목록
        List<ANode> CloseList = new List<ANode>();


        // 정렬해주는 메서드 / 출력할 때 작은 숫자 순서대로 나오나?
        // openList 에서 우선순위가 제일 높은 것을
        ANode getNodeInOpenList()
        {
            ANode target = null;
            // openList를 F score 기준으로 정렬 [오름차순] - 이렇게 하면 openList 의 0번째가 우선순위가 제일 높은 것이다.
            OpenList.Sort((a, b) => a.F.CompareTo(b.F));// 어떤 걸 기준으로 정렬을 할지 클래스를 정렬해야하는데? -> 
            // a의 Fscore와 b의 Fscore를 비교해서 / Sort 다음에 람다식 또는 함수를 사용
            target = OpenList[0]; // 우선순위 제일 높은 것을 전달해 주기 위해서 참조변수로...

            OpenList.RemoveAt(0); // 맨 앞에 것을 꺼내는 효과를 위해...
            return target;
        }


        // 출발점 : sy, sx / 도착점 : ey, ex
        void FindPath(int sy, int sx, int ey, int ex)
        {
            int gScore = 0;
            ANode curNode = new ANode();
            ANode startNode = myMaps[sy, sx];
            ANode endNode = myMaps[ey, ex];
            ANode targetNode = new ANode();


            // 시작 지점을 Open List 에 넣는다.
            OpenList.Add(startNode);

            // 시작 지점을 방문했다고 표시한다. (Closed List에 넣으라는 의미)
            //CloseList.Add(startNode);

            // 반복 시작 - 앞으로 갈 지점이 없을 때까지 (Open List에 Node가 하나도 없다는 뜻)
            while (OpenList.Count > 0)
            {
                // Open List에서 노드 하나를 꺼내 온다.
                // 앞으로 갈 수 있는 Node 중에서 F score가 가장 낮은 (비용이 가장 낮은) Node를 선택한다는 뜻 
                curNode = getNodeInOpenList();
                // 위에서 꺼낸 노드를 방문했다고 기록한다.
                // (Closed List에 추가한다는 뜻)
                CloseList.Add(curNode);

                // 위에서 꺼낸 노드가 도착점인가 ??? 도착점이면 바로 반복문 종료
                if (curNode.X == ex && curNode.Y == ey) // if(curNode == endNode) 
                {


                    // 찾은 경로 표시하기

                    // 반복문을 통해 도착점의 부모노드를 따라서 시작점까지 이동하면서 Node를 기록하고

                    // 경로를 출력한다.
                    do
                    {
                        findedPath.AddFirst(curNode);

                    } while (curNode=curNode.Parent);
                    break;
                }

                ////// 또 반복 시작 : 위에서 꺼낸 Node를 중심으로 주변 Node들에게 다음 루틴을 반복 적용한다.
                // 현재 노드 중심으로 (좌상, 상, 우상, 좌, 우, 좌하, 하, 우하) 주변 8군데 체크                
                OpenList.Add(myMaps[sx - 1, sy - 1]);   // 좌상
                OpenList.Add(myMaps[sx, sy - 1]);       // 상
                OpenList.Add(myMaps[sx + 1, sy - 1]);   // 우상

                OpenList.Add(myMaps[sx - 1, sy]);       // 좌
                OpenList.Add(myMaps[sx + 1, sy]);       // 우

                OpenList.Add(myMaps[sx - 1, sy + 1]);   // 좌하
                OpenList.Add(myMaps[sx, sy + 1]);       // 하
                OpenList.Add(myMaps[sx + 1, sy + 1]);   // 우하

                int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };



                for (int i = 0; i < 8; i++)
                {
                    ////// 배열 범위 내에 있는지 체크 : 범위를 벗어나면 다음 주변 노드로 {continue 하라는 뜻}
                    if (curNode.Y + dy[i] < 0 || curNode.Y + dy[i] >= map_height) continue;
                    if (curNode.X + dx[i] < 0 || curNode.X + dx[i] >= map_width) continue;
                    // dx[i] 를 보면 대각선 이동인지 확인할 수 있다.

                    // 주변 노드
                    targetNode = myMaps[curNode.Y + dy[i], curNode.X + dx[i]];

                    ////// 갈 수 있는 노드인지 체크 (장애물 체크 : ANode의 IsBlock을 체크하라는 뜻)            
                    if (targetNode.IsBlock) continue;

                    ////// 이미 방문했는지 체크(closed list 에서 체크하라는 뜻)
                    if (CloseList.Contains(targetNode)) continue;


                    // G score 계산
                    if (Math.Abs(dx[i]) + Math.Abs(dy[i]) == 2)
                        gScore = curNode.G + 14;
                    else
                    {
                        gScore = curNode.G + 10;
                    }

                    ////// 주변 노드가 Open List 에 추가되어 있는지 체크
                    if (OpenList.Contains(targetNode) == false)
                    {
                        ////// Open List 에 없다면...
                        ////// 주변 노드에 G score 계산, H score 계산
                        ////// G score 계산 시, 현재 Node에 부모가 되는 Node에 G score 도 포함해서 계산하라는 뜻
                        ////// H score 는 도착점과의 거리(배열의 칸수를 계산해도 됨)를 기록
                        targetNode.G = gScore;
                        targetNode.H = Math.Abs(ey - targetNode.Y) + Math.Abs(ex - targetNode.X);

                        ////// 주변 Node의 부모를 현재 Node로 기록
                        targetNode.Parent = curNode;

                        ////// Open List에 주변 노드 추가
                        OpenList.Add(targetNode);

                    }
                    else
                    {
                        ////// Open List 에 있다면...
                        ////// 현재 Node를 기준으로 G score를 계산해서 기존의 G score를 비교해서 새로 계산 G score 가 작으면 Update
                        if (targetNode.G > gScore)
                        {
                            targetNode.G = gScore;
                            targetNode.Parent = curNode;
                        }
                        ////// 여기서 만약 Update 된다면, 부모도 현재 Node로 바꿔 준다.

                    }


                }


                // 메모리 차이?
                /*for (int y = 0; y <= 8; ++y)
                {
                    for (int x = 1; x <= 1; ++x)
                    {

                    }
                }*/





            }



            // 가중치 넣기


            // 위에서 꺼낸 Node를 방문했다고 기록한다. (Close List에 추가한다는 뜻)
            for (int i = 0; i < OpenList.Count; i++)
            {
                CloseList.Add(OpenList[i]);
            }
            // 이미 CloseList에 있는 애면 탐색 안함

        }



    }

}
// Open List 에서 Node 하나를 꺼내 온다.

// 앞으로 갈 수 있는 Node 중에서 F score가 가장 낮은 (비용이 가장 낮은) Node를 선택한다는 뜻 
//curNode = getNodeInOpenList();
// 위에서 꺼낸 Node를 방문했다고 기록한다.
// (Close List에 추가한다는 뜻)

// 위에서 꺼낸 노드가 도착점인가? 도착점이면 바로 반복문 종료

////// 또 반복 시작 : 위에서 꺼낸 Node를 중심으로 주변 Node들에게 다음 루틴을 반복 적용한다.
////// 배열 범위 내에 있는지 체크 : 범위를 벗어나면 다음 주변 노드로 {continue 하라는 뜻}
////// 갈 수 있는 노드인지 체크 (장애물 체크 : ANode의 IsBlock을 체크하라는 뜻)            
////// 이미 방문했는지 체크(closed list 에서 체크하라는 뜻)

////// 주변 노드가 Open List 에 추가되어 있는지 체크
////// Open List 에 없다면...
////// 주변 노드에 G score 계산, H score 계산
////// G score 계산 시, 현재 Node에 부모가 되는 Node에 G score 도 포함해서 계산하라는 뜻
////// H score 는 도착점과의 거리(배열의 칸수를 계산해도 됨)를 기록
////// 주변 Node의 부모를 현재 Node로 기록
////// Open List에 주변 노드 추가

////// Open List 에 있다면...
////// 현재 Node를 기준으로 G score를 계산해서 기존의 G score를 비교해서 새로 계산 G score 가 작으면 Update
////// 여기서 만약 Update 된다면, 부모도 현재 Node로 바꿔 준다.

// 반복 시작 - 앞으로 갈 지점이 없을 때까지 (Open List에 Node가 하나도 없다는 뜻)


// 큐 : 선입선출
// 우선탐색 큐 : 우선순위대로 출력

// 정렬 오름차순하면 첫번째가 가장 작은 값이 되겠지?
// 그걸 딱 꺼내면 되는거임


// 우선 순위 큐 만들기
// PriorityQueue<ANode, int> openList = new PriorityQueue<ANode, int>;
// openList.enqueue(Node, ANode 의 Fscore);
