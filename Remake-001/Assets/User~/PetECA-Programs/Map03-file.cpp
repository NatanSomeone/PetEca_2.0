#include "petecavirtual.h"

using namespace std;
int Main(){
	
	cout<<"Olá, bem vindo, aplicação comecando agora"<<endl;
	cout<<"..."<<endl;Sleep(1000);

		LOG = TRUE;
		
		GameSpeed(0.8);
		Restart();
		CameraSel(0);
		Move(1);
	for(int i =0;i<1;i++) {
		Move(3);
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
//		Claw();
//		Rotate(180);
//		Move(6);
//		Rotate(180);
		}
		
		DoLOOP= TRUE;
		Finished= TRUE;		
		
	
}
