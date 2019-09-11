#include "petecavirtual.h"

using namespace std;
int Main(){
	
	cout<<"Hi, welcome the emulation is starting now."<<endl;
	cout<<"..."<<endl;Sleep(1000);

		LOG = TRUE;
		Wait(1);	
		Rotate(-90);
		Move(1);
		Rotate(90);
		Move(1);
		
		Wait(5);	
		for(int j=0;j<100;j++)
		{
			Wait(2);
			Move(3);
			for(int i = 0;i<3;i++){
				Rotate(-90);
				Move(3);
				Claw();
				Wait(1);
				Claw();
				Sleep(2000);
			}
			Rotate(-90);
			Sleep(5000);
		}
		
		DoLOOP= TRUE;
		Finished= TRUE;		
		
	
}
