#include "petecavirtual.h"

using namespace std;
int Main(){
	
	cout<<"Hi, welcome the emulation is starting now."<<endl;
	cout<<"..."<<endl;Sleep(1000);

		LOG = TRUE;
			
		Wait(5);
		//cout<<GetTime()<<(char)7;
		for(int j=0;j<4;j++)
		{
			Wait(2);
			Move(3);
			for(int i = 0;i<3;i++){
				Rotate(-90);
				Move(8);
				Claw();
				Wait(1);
				Claw();
			}
			Rotate(-90);
		}
		
		DoLOOP= TRUE;
		Finished= TRUE;		
		
	
}
