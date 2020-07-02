#include "petecavirtual.h"
float f, T_P, T_QRS, T_T, T_U, T_PR, T_ST, t_P, t_QRS, t_T, t_U, A_P, A_QRS, A_T, A_U, w_P, w_QRS, w_T, w_U;
using namespace std;
int Main()
{

	cout << "Olá, bem vindo, aplição comecando agora" << endl;
	cout << "..." << endl;
	Sleep(1000);

	LOG = TRUE;

	Restart();
	Rotate(-90);
	Move(2);
	Rotate(90);
	Move(2);

	Wait(5);
	for (int j = 0; j < 4; j++)
	{
		Wait(2);
		Move(3);
		for (int i = 0; i < 3; i++)
		{
			Rotate(-90);
			Move(3);
			Claw();
			Wait(1);
			Claw();
		}
		Rotate(-90);
	}

	DoLOOP = TRUE;
	Finished = TRUE;
}
