using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public partial class LevelManager : Node2D
{
	// Randomized number of vertical walls
	int tWalls;
	int bWalls;
	int width;

	int[] T;
	int[] tL;
	int[] B;
	int[] bL;

	// Randomized number of horizontal walls
	int lWalls;
	int rWalls;
	int height; 

	int[] L;
	int[] lL;
	int[] R;
	int[] rL;

	// reference point where to pull floor tile
	Vector2I reference = new Vector2I(9,0); 

	int [,] occupied;

	int lastCross;
	
	[Export]
	public Godot.TileMap map;

	private PackedScene mapScene = (PackedScene) ResourceLoader.Load("res://World/OcclusionTileMap.tscn");
	private PackedScene keyNode = (PackedScene) ResourceLoader.Load("res://World/Key.tscn");
	private PackedScene clownNode = (PackedScene) ResourceLoader.Load("res://World/ClownJail.tscn");
	private PackedScene enemy = (PackedScene) ResourceLoader.Load("res://Enemies/BasicGuard/basicGuard.tscn");
	private PackedScene safeNode = (PackedScene) ResourceLoader.Load("res://World/ItemSafe.tscn");
	private PackedScene exit = (PackedScene) ResourceLoader.Load("res://World/Exit.tscn");
	//private PackedScene jailNode = (PackedScene) ResourceLoader.Load("res://World/JailCell.tscn");

	const int numOptions = 3;
	int[,,] setUpArray;

	int doorFreq = 30;

	HashSet<int> vertSpots = new HashSet<int>();
	HashSet<int> horSpots = new HashSet<int>();

	int maxLevel = 3;
	Dictionary<int,int> clownsPerLevel = new Dictionary<int,int>();
	Dictionary<int,int> requiredClownsFreed = new Dictionary<int,int>();
	Dictionary<int,int> guardsPerLevel = new Dictionary<int,int>();
	private int currentClownsFreed;
	int currentGuardsGoofed;



	public void buildWalls(int xWalls, int[] xL, int[] X, HashSet<int> checkSet, int dim) {
		int last = 0;

		var random = new Random();

		for (int i=0; i<xWalls; i++) {
			int temp = random.Next(last + 7, last + 20);
			while (checkSet.Contains(temp-2) || checkSet.Contains(temp-1) || 
				checkSet.Contains(temp) || checkSet.Contains(temp+1) || checkSet.Contains(temp+2)){

				temp = random.Next(last + 7, last + 20);
			}
			checkSet.Add(temp);
			X[i] = temp;
			last = temp;
			temp = random.Next(5, dim-2);
			xL[i] = temp;
		}
	}

	public bool suitableZoneSize(int x, int y) {
		int[] zone = findZone(x,y);

		return ((zone[0]+zone[1] >= 3 && zone[2] >=1 && zone[3] >= 1) || (zone[0] >= 1 && zone[1] >= 1 && zone[2] + zone[3] >= 3));
	}

	public void setCells(int x, int y, int val, bool wall) {
		//
		if (wall) {
			map.SetCellsTerrainConnect(0,new Godot.Collections.Array<Vector2I>{new Vector2I(x,y)},0,0);
		} else {
			map.SetCell(0, new Vector2I(x,y),val, reference);
		}
		occupied[x,y]=val;
		// got an index out of bounds here
		// in drawLine from call that looks like this
		// for (int i = x-d; i <= x+u; i++) {
		// 	setCells(x,i,1,true);
		// }
	}

	public void checkNear(int x, int y) {
		if (occupied[x,y] != 3) {
			setCells(x, y,3,false);
		}
	}

	public void drawLine(int x, int y, int[] zone) {
		var random = new Random();
		int dim = random.Next(0,2);

		if (dim == 0) { // go horizontal
			int l = random.Next(0, zone[0]);
			int r = random.Next(0, zone[1]);

			for (int i = x-l; i <= x+r; i++) {
				setCells(i,y,1,true);
			}
		} else { // go vertical
			int u = random.Next(0, zone[2]);
			int d = random.Next(0, zone[3]);

			for (int i = x-d; i <= x+u; i++) {
				setCells(x,i,1,true);
			}
		}
	}

	public void drawCloset(int x, int y, int[] zone) {
		/*var random = new Random();

		int type = random.Next(0, 4); // which way is opening facing

		int hor = 0;
		int ver = random.Next(0, zone[2]);;

		if (type == 0) {
			hor = random.Next(0, zone[0]);
			
		}*/
	}

	public void drawLittleL(int x, int y, int[] zone) {
		var random = new Random();

		int lr = random.Next(0, 2);
		int ud = random.Next(0, 2);

		int temp;
		if (lr == 0) {
			temp = random.Next(0,zone[0]);
			for (int i = x-temp; i <= x; i++) {
				setCells(i,y,1,true);
			}
		} else {
			temp = random.Next(0,zone[1]);
			for (int i = x+temp; i >= x; i--) {
				setCells(i,y,1,true);
			} 
		}

		if (ud == 0) {
			temp = random.Next(0,zone[2]);
			for (int i = y+temp; i >= y; i--) {
				setCells(x,i,1,true);
			}
		} else {
			temp = random.Next(0,zone[3]);
			for (int i = y-temp; i <= y; i++) {
				setCells(x,i,1,true);
			} 
		}		

		
	}

	public void drawClump(int x, int y, int[] zone) {
		var random = new Random();

		int subL = random.Next(0, zone[0]);
		int subR = random.Next(0, zone[1]);
		int subU = random.Next(0, zone[2]);	
		int subD = random.Next(0, zone[3]);

		GD.Print("Let's try");
		for (int i = x-subL; i <= x+subR; i++) {
			for (int j = y-subD; j <= y+subU; j++) {
				int place = random.Next(0,90);
					if (place < 15) {
						GD.Print("Placing");
						GD.Print(i);
						GD.Print(j);
						setCells(i,j,1,true);
					} else {
						GD.Print("No print");
					}
			}
		}
	}

	public void drawBox(int x, int y, int[] zone) {
		var random = new Random();


		int left = random.Next(0, zone[0]);
		int right = random.Next(0, zone[1]);
		int up = random.Next(0, zone[2]);
		int down = random.Next(0, zone[3]);

		GD.Print(left);
		GD.Print(right);
		GD.Print(up);
		GD.Print(down);

		for (int i = x-left; i < x+right; i++) {
			for (int j = y-down; j <= y+up; j++) {
				setCells(i,j,1,true);
			}
		}
	}

	public void drawPlus(int x, int y, int[] zone) {

	}

	public int[] findZone(int x, int y) {
		int[] toRet = new int [4]; //l,r,u,d

		int i = 0;
		while (i < 10 && occupied[x-i,y] == 3 ) {
			i++;
		}
		i = Math.Max(-1, i-2);
		toRet[0] = i;

		i = 0;
		while (i < 10 && occupied[x+i,y] == 3) {
			i++;
		}
		i = Math.Max(-1, i-2);
		toRet[1] = i;

		i = 1;
		while(i<10){
			bool done = false;
			for (int j = x-toRet[0]; j < x+toRet[1]; j++) {
				if (occupied[j,y+i] != 3) {
					done = true;
					break;
				}
			}
			if (done) {
				break;
			}
			i++;
		}
		
		i = Math.Max(-1, i-2);
		toRet[2] = i;

		i = 1;
		while(i<10){
			bool done = false;
			for (int j = x-toRet[0]; j < x+toRet[1]; j++) {
				if (occupied[j,y-i] != 3) {
					done = true;
					break;
				}
			}
			if (done) {
				break;
			}
			i++;
		}
		i = Math.Max(-1, i-2);
		toRet[3] = i;

		GD.Print("For");
		GD.Print(x);
		GD.Print(y);
		GD.Print("It's");
		GD.Print(toRet[0]);
		GD.Print(toRet[1]);
		GD.Print(toRet[2]);
		GD.Print(toRet[3]);

		return toRet;
	}

	public void setupLevel(int iteration) {
		GD.Print("Setting up");
		resetCurrentLevelStats();
		// clear last node with map in it if it exists
		Node2D levelNode = GetNodeOrNull<Node2D>("LevelNode");

		if (map != null){
			levelNode.RemoveChild(map);
			map.QueueFree();
		}

		if (levelNode != null){
			RemoveChild(levelNode);
			levelNode.QueueFree();
		}

		// make new node for level and add map to it
		levelNode = new Node2D();
		levelNode.Name = "LevelNode";
		map = mapScene.Instantiate<TileMap>();
		levelNode.AddChild(map);
		AddChild(levelNode);
		
		// Make global*(many are)***********************
		var random = new Random();
		double scalar = (1+iteration*0.3);
		width = random.Next((int)(30*scalar),(int)(45*scalar));
		height = random.Next((int)(30*scalar),(int)(45*scalar));

		vertSpots = new HashSet<int>();
		horSpots = new HashSet<int>();

		vertSpots.Add(0);
		vertSpots.Add(width-1);

		horSpots.Add(0);
		horSpots.Add(height-1);

		// Maybe make these dependent-ish on width/height
		tWalls = random.Next(2,4);
		bWalls = random.Next(6-tWalls,8-tWalls);
		lWalls = random.Next(2,4);
		rWalls = random.Next(6-lWalls,8-lWalls);

		T = new int[tWalls];
		tL = new int[tWalls];
		B = new int[bWalls];
		bL = new int[bWalls];
		L = new int[lWalls];
		lL = new int[lWalls];
		R = new int[rWalls];
		rL = new int[rWalls];

		Vector2I [] corners = new Vector2I[(tWalls+bWalls)*(lWalls+rWalls)];
		occupied = new int[2*width,2*height]; //0=empty,1=wall,-1=door,3=floor
		for (int i = 0; i < 2*width;i++) {
			for (int j = 0; j<2*height;j++) {
				occupied[i,j] = 3;
			}
		}
		//int cornerCounter = 0;

		//*********************************************

		// Can all be a function, pass value of xWalls and arrays X and xL (make sure pointers work)
		// Do something with sets to prevent walls being too close
		// GD.Print("ew");
		
		buildWalls(tWalls, tL, T, vertSpots, height);
		buildWalls(lWalls, lL, L, horSpots, width);
		buildWalls(bWalls, bL, B, vertSpots, height);
		buildWalls(rWalls, rL, R, horSpots, width);

		//**********************************************************
		
		// Setup floor
		for (int i=0;i<height;i++) {
			for (int j=0;j<width;j++) {
				setCells(j,i,3,false);
			}
		}


		//Initialize map******************************
		for (int i = 0; i < height; i++) {
			setCells(0,i,1,true);
			setCells(width-1,i,1,true);
		}
		for (int i = 0; i < width; i++) {
			setCells(i,0,1,true);
			setCells(i,height-1,1,true);
		}
		//********************************************************

		// Build walls***********************
		for (int i = 0; i < tWalls; i++) {
			for (int j=0; j< tL[i]; j++) {
				if (random.Next(1,doorFreq) != 1) {
					setCells(T[i],j,1,true);
				}
			}
		}

		Vector2I tempCell;
		for (int i = 0; i < lWalls; i++) {
			lastCross = 0;
			for (int j=0; j< lL[i]; j++) {
				//tempCell = map.GetCellAtlasCoords(0, new Vector2I(j,L[i]));
				if (occupied[j,L[i]] != 3 && j != 0) {

					//step back and open a door
					int temp = random.Next(lastCross+1, j-1);
					lastCross = j;
					setCells(temp,L[i],3,false);

					temp = random.Next(0, 10);
					if (temp < 5) {
						lL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(j,L[i]+1);
					checkNear(j,L[i]-1);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(j,L[i],1,true);
				}
			}

			
			checkNear(lL[i]+1, L[i]+1);
			checkNear(lL[i]+1, L[i]-1);
		}
		for (int i = 0; i < bWalls; i++) {
			lastCross = 0;
			for (int j=0; j< bL[i]; j++) {
				//tempCell = map.GetCellAtlasCoords(0, new Vector2I(B[i],height-1-j));
				if (occupied[B[i],height-1-j] != 3 && j != 0) {

					//step back and open a door
					int temp = random.Next(1,j-1-lastCross);
					lastCross = j;
					setCells(B[i],height-1-j+temp,3,false);

					temp = random.Next(0, 10);
					if (temp < 5) {
						bL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(B[i]+1,height-1-j);
					checkNear(B[i]-1,height-1-j);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(B[i],height-1-j,1,true);
				}
			}

			checkNear(B[i]-1,height - bL[i]+1);
			checkNear(B[i]+1,height - bL[i]+1);
		}
		for (int i = 0; i < rWalls; i++) {
			lastCross = 0;
			for (int j=0; j< rL[i]; j++) {
				//tempCell = map.GetCellAtlasCoords(0, new Vector2I(width-1-j,R[i]));
				if (occupied[width-1-j,R[i]] != 3 && j != 0) {

					//step back and open a door
					int temp = random.Next(1,j-1-lastCross);
					lastCross = j;
					setCells(width-1-j+temp,R[i],3,false);

					temp = random.Next(0, 10);
					if (temp < 4) {
						rL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(width-1-j,R[i]+1);
					checkNear(width-1-j,R[i]-1);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(width-1-j,R[i],1,true);
				}
			}

			
			checkNear(width-rL[i]-1, R[i]-1);
			checkNear(width-rL[i]-1, R[i]+1);
		}
		//**************************************************
		
		int numGarbage = width*height / 100;
		GD.Print("Garbage day");
		GD.Print(numGarbage);

		while (numGarbage > 0) {
			GD.Print("Garbage");
			GD.Print(numGarbage);
			int failureCount = 0;

			int tempX=0;
			int tempY=0;

			int[] zone = new int[4];

			while (failureCount < 100) {
				tempX = random.Next(0, width-1);
				tempY = random.Next(0, height-1);
				zone = findZone(tempX, tempY);

				if (zone[0] > 0 && zone[1] > 0 && zone[2] > 0 && zone[3] > 0) {
					break;
				} else {
					GD.Print("Fail");
					failureCount++;
				}
			}
			if (failureCount < 100) {
				int garbageType = random.Next(0,100);

				if (garbageType <= 20) {
					drawLine(tempX,tempY,zone);
				} else if (garbageType <= 40) {
					drawLittleL(tempX,tempY,zone);
				} else if (garbageType <= 60) {
					drawBox(tempX,tempY,zone);
				} else if (garbageType <= 70) {
					drawClump(tempX,tempY,zone);
				}

				numGarbage--;
			} else {
				break;
			}
		}
		// Add items*****************


		// Keys
		
		int numKeyClowns = clownsPerLevel[iteration];
		GD.Print("Num keys");
		GD.Print(numKeyClowns);

		for (int i = 0; i < numKeyClowns; i++) {
			int x = random.Next(0, width);
			int y = random.Next(0, height);

			while (!suitableZoneSize(x,y)) {
				x = random.Next(0, width);
				y = random.Next(0, height);
			}

			var newKey = (Key) keyNode.Instantiate();
			newKey.Position = new Vector2(16*x, 16*y);
			levelNode.AddChild(newKey);
		}
		GD.Print("Added");
		// Add clowns
		for (int i = 0; i < numKeyClowns; i++) {
			int x = random.Next(0, width);
			int y = random.Next(0, height);

			while (!suitableZoneSize(x,y)) {
				x = random.Next(0, width);
				y = random.Next(0, height);
			}

			var newClown = (ClownJail) clownNode.Instantiate();
			newClown.Position = new Vector2(16*x, 16*y);
			levelNode.AddChild(newClown);
		}

		// Add safe
		for (int i = 0; i < 1; i++) {
			int x = random.Next(0, width);
			int y = random.Next(0, height);

			while (!suitableZoneSize(x,y)) {
				x = random.Next(0, width);
				y = random.Next(0, height);
			}

			var newSafe = (ItemSafe) safeNode.Instantiate();
			newSafe.Position = new Vector2(16*x, 16*y);
			levelNode.AddChild(newSafe);
		}

		// Exit
		for (int i = 0; i < 1; i++) {
			int x = random.Next(0, width);
			int y = random.Next(0, height);

			while (!suitableZoneSize(x,y)) {
				x = random.Next(0, width);
				y = random.Next(0, height);
			}

			var newExit = (Exit) exit.Instantiate();
			newExit.currentLevel = iteration;
			newExit.Position = new Vector2(16*x, 16*y);
			levelNode.AddChild(newExit);
		}

		// Add enemies
		// int numEnemies = (int)scalar*random.Next(8,12);
		int numEnemies = guardsPerLevel[iteration];
		GD.Print(numEnemies);

		for (int i = 0; i < numEnemies; i++) {
			int x = random.Next(0, width);
			int y = random.Next(0, height);

			while (!suitableZoneSize(x,y)) {
				x = random.Next(0, width);
				y = random.Next(0, height);
			}

			var nme = (BasicGuardController) enemy.Instantiate();
			nme.Position = new Vector2(16*x, 16*y);

			// nme.moveSpeed = 3000;
			levelNode.AddChild(nme);
		}
	}

	private void tryEndLevel(int currentLevel){
		if (currentClownsFreed >= requiredClownsFreed[currentLevel]){
			// separate entire level into child node of the level manager
			// free that node at level end
			// make player invisible, show end level gui
			// when player moves on from gui, generate new level in the same way with a child node and begin again

			// show player stats and present option to move on to next level, which will call setupLevel
			GetTree().CallGroup("end_level_gui", "endLevel", currentLevel);
			GD.Print(currentClownsFreed + ", " + currentGuardsGoofed);

			// this should be in the end level gui
			
			if (currentLevel <= maxLevel){
				// make new level
				setupLevel(currentLevel+1);
			} else {
				// end game
				GetTree().ChangeSceneToFile("res://GUI/MainMenu.tscn");
			}
		}
		
	}

	private void resetCurrentLevelStats(){
		currentClownsFreed = 0;
		currentGuardsGoofed = 0;
	}

	// to be called by guard when killed
	private void guardGoofed(){
		currentGuardsGoofed++;
	}

	// to be called by clown jail cell when freed
	private void clownFreed(){
		currentClownsFreed++;
	}
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clownsPerLevel.Add(1, 1);
		clownsPerLevel.Add(2, 3);
		clownsPerLevel.Add(3, 5);

		requiredClownsFreed.Add(1, 1);
		requiredClownsFreed.Add(2, 2);
		requiredClownsFreed.Add(3, 3);

		guardsPerLevel.Add(1, 10);
		guardsPerLevel.Add(2, 15);
		guardsPerLevel.Add(3, 20);

		setupLevel(1);
	}

}
