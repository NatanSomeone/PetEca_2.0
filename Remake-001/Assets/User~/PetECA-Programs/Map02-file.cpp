#include "petecavirtual.h"

using namespace std;
int Main(){
	
	cout<<"Olá, bem vindo, aplicação comecando agora"<<endl;
	cout<<"..."<<endl;Sleep(1000);

		LOG = TRUE;
		
		GameSpeed(8);
		Restart();
		CameraSel(0);
	float t=5;
	while(t>0) {
		Move(4);
		Rotate(90);
		Move(8);
		Rotate(90);
		Move(3);
		Claw();
		Rotate(90);
		Move(3);
		Claw();
		Rotate(180);
		Move(3);
		Rotate(-90);
		Move(3);
		Rotate(-90);
		Move(8);
		Rotate(90);
		Move(3);
		Claw();
		Claw();
		Rotate(180);
		Move(7);
		Rotate(-180);
		t=GetTime();
		cout<<"Time:"<<t;
		}
		
		DoLOOP= TRUE;
		Finished= TRUE;		
		
	
}
