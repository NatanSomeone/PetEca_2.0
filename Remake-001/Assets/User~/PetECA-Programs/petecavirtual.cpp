#include "math.h"
using namespace std;
int Main(){
	LOG = FALSE;
	
	LOG = TRUE;
		Restart();		
		GameSpeed(2);
		cout<<GetTime()<<(char)7;
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
		for(int i =0 ; i<1; i++)
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
		Restart();
	
	DoLOOP= TRUE;
	//Finished= TRUE;	
}
