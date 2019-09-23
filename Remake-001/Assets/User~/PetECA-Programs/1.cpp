#include "petecavirtual.h"
#include "math.h"
using namespace std;
int Main(){
	
	cout<<"Hi, welcome the emulation is starting now."<<endl;
	cout<<"..."<<endl;Sleep(1000);
	float t0;	
		SelRobot(0);
		
		LOG = true;
		GameSpeed(10);
		Restart();
//		while(TestSensor(0)==0){
//			Move(0.2);
//		}
		
		t0 = GetTime(); //InitialTime
		CameraSel(0); //top view
		Wait(5);
		CameraSel(2); //car view
		Rotate(-45);
		float v= 7.4;
		Claw();
		Move(sqrt(2*v*v));
		Claw();
		Move(0.75);
		Rotate(-135);
		Move(6.5);
		Claw();
		Wait(1);
		for(int i =0 ; i<15; i++)
		{
			Rotate(-180);
			Move(5.75);
			Claw();
			Rotate(180);
			Move(5.75);
			Claw();
			Wait(0.2);	
		}
		CameraSel(1); //defaultView
		Claw();
		Move(1.25);
		Rotate(-90);
		Move(7.75);
		Rotate(-90);
		GameSpeed(1);	
		Wait(1);
//		cout<<"It lasted "<<GetTime()-t0<<" seconds , and was earned "<<GetScore()<<" points"<<endl;
//		Restart();
		
		
		DoLOOP= TRUE;
		Finished= TRUE;		
		
	
}

/******************Dados do mapa*****************
3 Robos------------------------------------------
 (0)	-Cor: Vermelho	Pos: (x,y) Garra: (x,y)
 (1)	-Cor: Verde		Pos: (x,y) Garra: (x,y)
 (2)	-Cor: Azul		Pos: (x,y) Garra: (x,y)
4 Cameras----------------------------------------
 (0)	-Interna do robo
 (1)	-Interna do robo
 (2)	-Vista Superior
 (3)	-Vista 1
7 Pontos de Referencia--------------------------
 (0)	-Pos: (x,y)		-[FonteDeVermelho]
 (1)	-Pos: (x,y)		-[FonteBranca]
 (2)	-Pos: (x,y)		-[CheckPoint]
 (3)	-Pos: (x,y)		-[#2]
 (4)	-Pos: (x,y)		-[#1]
 (5)	-Pos: (x,y)		-[#3]
 (6)	-Pos: (x,y)		-[PontoDeColeta]
 
******************Dados do mapa*****************/
