#include "petecavirtual.h"
#include "math.h"
using namespace std;
int Main(){
	
	cout<<"Hi, welcome the emulation is starting now."<<endl;
	cout<<"..."<<endl;Sleep(1000);

		LOG = TRUE;
		
		float v;
		cin>>v;
		Rotate(v);
		
		
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
