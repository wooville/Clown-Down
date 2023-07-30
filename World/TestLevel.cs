using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public partial class TestLevel : Node2D
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

	// reference point where to pull wall tile
	Vector2I reference = new Vector2I(0,0); 

	int [,] occupied;

	int lastCross;
	
	[Export]
	public Godot.TileMap map;

	const int numOptions = 3;
	int[,,] setUpArray;

	int doorFreq = 40;

	HashSet<int> vertSpots = new HashSet<int>();
	HashSet<int> horSpots = new HashSet<int>();

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
			temp = random.Next(5, dim-1);
			xL[i] = temp;
		}
	}

	public void setCells(int x, int y, int val) {
		map.SetCell(0, new Vector2I(x,y),val, reference);
		occupied[x,y]=val;
	}

	public void checkNear(int x, int y) {
		if (occupied[x,y] == 1 ) {
			setCells(x, y,-1);
		}
	}

	public void drawLine(int x, int y, int[] zone) {
		var random = new Random();
		int dim = random.Next(0,2);

		if (dim == 0) { // go horizontal
			int l = random.Next(0, zone[0]);
			int r = random.Next(0, zone[1]);

			for (int i = x-l; i <= x+r; i++) {
				setCells(i,y,1);
			}
		} else { // go vertical
			int u = random.Next(0, zone[2]);
			int d = random.Next(0, zone[3]);

			for (int i = x-d; i <= x+u; i++) {
				setCells(x,i,1);
			}
		}
	}

	public void drawCLine(int x, int y, int[] zone) {

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
						setCells(i,j,1);
					} else {
						GD.Print("No print");
					}
			}
		}
	}

	public void drawBox(int x, int y, int[] zone) {

	}

	public void drawPlus(int x, int y, int[] zone) {

	}

	public int[] findZone(int x, int y) {
		int[] toRet = new int [4]; //l,r,u,d

		int i = 0;
		while (i < 10 && occupied[x-i,y] == 0) {
			i++;
		}
		i = Math.Max(-1, i-2);
		toRet[0] = i;

		i = 0;
		while (i < 10 && occupied[x+i,y] == 0) {
			i++;
		}
		i = Math.Max(-1, i-2);
		toRet[1] = i;

		i = 1;
		while(i<10){
			bool done = false;
			for (int j = x-toRet[0]; j < x+toRet[1]; j++) {
				if (occupied[j,y+i] != 0) {
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
				if (occupied[j,y-i] != 0) {
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Make global*(many are)***********************
		var random = new Random();
		width = random.Next(40,60);
		height = random.Next(40,60);

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
		occupied = new int[2*width,2*height]; //0=empty,1=wall,-1=door
		//int cornerCounter = 0;

		//*********************************************

		// Can all be a function, pass value of xWalls and arrays X and xL (make sure pointers work)
		// Do something with sets to prevent walls being too close
		
		buildWalls(tWalls, tL, T, vertSpots, height);
		buildWalls(lWalls, lL, L, horSpots, width);
		buildWalls(bWalls, bL, B, vertSpots, height);
		buildWalls(rWalls, rL, R, horSpots, width);

		//**********************************************************

		//Initialize map******************************
		for (int i = 0; i < height; i++) {
			setCells(0,i,1);
			setCells(width-1,i,1);
		}
		for (int i = 0; i < width; i++) {
			setCells(i,0,1);
			setCells(i,height-1,1);
		}
		//********************************************************

		// Build walls***********************
		for (int i = 0; i < tWalls; i++) {
			for (int j=0; j< tL[i]; j++) {
				if (random.Next(1,doorFreq) != 1) {
					setCells(T[i],j,1);
				}
			}
		}

		Vector2I tempCell;
		for (int i = 0; i < lWalls; i++) {
			lastCross = 0;
			for (int j=0; j< lL[i]; j++) {
				tempCell = map.GetCellAtlasCoords(0, new Vector2I(j,L[i]));
				if (tempCell == reference && j != 0) {

					//step back and open a door
					int temp = random.Next(lastCross+1, j-1);
					lastCross = j;
					setCells(temp,L[i],-1);

					temp = random.Next(0, 10);
					if (temp < 5) {
						lL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(j,L[i]+1);
					checkNear(j,L[i]-1);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(j,L[i],1);
				}
			}

			
			checkNear(lL[i]+1, L[i]+1);
			checkNear(lL[i]+1, L[i]-1);
		}
		for (int i = 0; i < bWalls; i++) {
			lastCross = 0;
			for (int j=0; j< bL[i]; j++) {
				tempCell = map.GetCellAtlasCoords(0, new Vector2I(B[i],height-1-j));
				if (tempCell == reference && j != 0) {

					//step back and open a door
					int temp = random.Next(1,j-1-lastCross);
					lastCross = j;
					setCells(B[i],height-1-j+temp,-1);

					temp = random.Next(0, 10);
					if (temp < 5) {
						bL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(B[i]+1,height-1-j);
					checkNear(B[i]-1,height-1-j);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(B[i],height-1-j,1);
				}
			}

			checkNear(B[i]-1,height - bL[i]+1);
			checkNear(B[i]+1,height - bL[i]+1);
		}
		for (int i = 0; i < rWalls; i++) {
			lastCross = 0;
			for (int j=0; j< rL[i]; j++) {
				tempCell = map.GetCellAtlasCoords(0, new Vector2I(width-1-j,R[i]));
				if (tempCell == reference && j != 0) {

					//step back and open a door
					int temp = random.Next(1,j-1-lastCross);
					lastCross = j;
					setCells(width-1-j+temp,R[i],-1);

					temp = random.Next(0, 10);
					if (temp < 4) {
						rL[i] = j+1;
					}
				} else if (j != 0){
					checkNear(width-1-j,R[i]+1);
					checkNear(width-1-j,R[i]-1);
				}
				if (random.Next(1,doorFreq) != 1) {
					setCells(width-1-j,R[i],1);
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
				int garbageType = random.Next(0, 2);

				switch (garbageType) {
					case 0:
						drawLine(tempX,tempY,zone);
						break;
					case 1:
						drawClump(tempX,tempY,zone);
						break;
					case 2:
						drawBox(tempX,tempY,zone);
						break;
					case 3:
						drawPlus(tempX,tempY,zone);
						break;

				}

				numGarbage--;
			} else {
				break;
			}
		}

		// Cover shit in garbage
		/*
		int numCover = (tWalls+bWalls+lWalls+rWalls);
		numCover = random.Next(numCover, 2*numCover);

		while (numCover > 0) {
			int coverType = random.Next(0, 3);
			
			if (coverType == 0) { // cluster
				int x = random.Next(1, width-1);
				int y = random.Next(1, height-1);

				if (occupied[x,y] == 0) {

				}
			} else if (coverType == 1) { // wall
				while (true) {
					int x = random.Next(1, width-1);
					int y = random.Next(1, height-1);

					int direction = random.Next(0,2);
					int max = random.Next(1,10);

					bool crossed = false;

					if (occupied[x,y] == 0) {
						while(max > 0) {
							if (direction == 0) {

							} else {

							}
							max--;
						}
						break;
					}
				}

			} else if (coverType == 2) { // T

			}

			numCover--;
		}*/
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ 
	}

}
