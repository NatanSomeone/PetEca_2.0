#include "petecavirtual.h"

using namespace std;
int Main(){
	
	LOG = TRUE;
	float v;
	cout<<"mover:";cin>>v;
	
	Move(v);
	Wait(1);
	
	
	getchar();
	DoLOOP= TRUE;
	Finished= TRUE;	
}
