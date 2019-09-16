#include <iostream>
#include <stdio.h>
#include <windows.h>
#include <string> 

#define Main() unsigned long __MAIN(void * pParam)
int Main();
unsigned long __stdcall NET_RvThr(void * pParam) ;

HANDLE hPipe1, hPipe2;
BOOL Finished;
BOOL DoLOOP;
BOOL LOG;

#define RED_ROBOT  0
#define BLUE_ROBOT 1

namespace editor
{
	char buf[100];  //sendByte
	char chBuf[100];//recieved byte
	
	BOOL WaitingForMsg = FALSE;
	int sRob = RED_ROBOT; //selectedROBOT
}

bool FocusChange(const char* WindowName);

int main()
{
	HANDLE hThread = NULL;
	
	LPTSTR lpszPipename1 = TEXT("\\\\.\\pipe\\PetServerPipe0");
	LPTSTR lpszPipename2 = TEXT("\\\\.\\pipe\\PetServerPipe1");

	Finished = FALSE;
	DoLOOP = FALSE;
	LOG = FALSE;

	hPipe1 = CreateFile(lpszPipename1, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
	hPipe2 = CreateFile(lpszPipename2, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);

	if ((hPipe1 == NULL || hPipe1 == INVALID_HANDLE_VALUE) || (hPipe2 == NULL || hPipe2 == INVALID_HANDLE_VALUE))
		printf("Could not connect, make sure the game window is open - (error %d)\n", GetLastError());
	else
	{
		CreateThread(NULL, 0, &__MAIN, NULL, 0, NULL);
		hThread = CreateThread( NULL, 0, &NET_RvThr, NULL, 0, NULL);
		system("title Peteca-console");
		FocusChange("Remake-001");//muda o foco para a janela do jogo

		while (!Finished) {}
		CloseHandle(hPipe1);
		CloseHandle(hPipe2);
	}

	getchar();
	system("cls");
	if (DoLOOP)	main();

	return 0;

}

void ReadPipes(char * out) {//trava se não receber dados
	DWORD cbRead;
	ReadFile(hPipe2, out, 100, &cbRead, NULL);
}

//CreateThread(NULL, 0, &ReadPipes, NULL, 0, NULL);
unsigned long ReadPipes(void * out) {//para leitura assincrona
	//std::cout<<"\""<<(char*)out<<"\"";
	ReadPipes(editor::chBuf);
}

///writes *out to the pipe
void WritePipes(char * out) {

	strcpy(editor::buf, out);
	
	if (strlen(editor::buf) > 0) {
		if (strcmp(editor::buf, "quit") != 0) {
			DWORD cbWritten;
			DWORD dwBytesToWrite = (DWORD)strlen(editor::buf);
			WriteFile(hPipe1, editor::buf, dwBytesToWrite, &cbWritten, NULL);
			if (LOG)
				printf("\n  (%d)\t- %s\n", cbWritten, editor::buf);
			Sleep(100);
		}
		else {
			Finished = TRUE;
		}
		memset(editor::buf, 0xcc, 100);

	}

}


///ReadAllMesagesThread
unsigned long __stdcall NET_RvThr(void * pParam) {
	BOOL fSuccess; 
	DWORD cbRead;
	int i;
	
	while(1)
	{
		char c[100];
		fSuccess =ReadFile( hPipe2,c,100,&cbRead, NULL); 
		if(fSuccess){
			strcpy(editor::chBuf, c);
			editor::WaitingForMsg=FALSE;
		}/*editor::chBuf Recieved*/
		if (! fSuccess && GetLastError() != ERROR_MORE_DATA) 
		{
			Finished = TRUE;//printf("Can't Read\n");
			editor::WaitingForMsg=FALSE;
			strcpy(editor::chBuf, new char[100]);
			break;
		}
	}
}


///Changes the focus to the specified window
bool FocusChange(const char* WindowName) {

	INPUT key;
	HWND hWnd = FindWindow(NULL, TEXT(WindowName));

	key.ki.wVk = VK_MENU;
	key.ki.dwFlags = 0;   //0 for key press
	SendInput(1, &key, sizeof(INPUT));

	SetForegroundWindow(hWnd); //sets the focus 

	key.ki.wVk = VK_MENU;
	key.ki.dwFlags = KEYEVENTF_KEYUP;  //for key release
	SendInput(1, &key, sizeof(INPUT));

	return (hWnd != NULL);
}


namespace {//editing area
#pragma region EDIT-AREA
#define W(X,...) char c[100];sprintf(c,X,## __VA_ARGS__);	WritePipes(c);
#define R(X,...) ed::WaitingForMsg = TRUE; W(X,## __VA_ARGS__); while(ed::WaitingForMsg){};
#define ed editor
#include <string>


	void SelRobot(int r) {
		ed::sRob = r;
	}
	void Move(float a) {
		//if(_buff == 1){	_buff == 2;}
		W("MOVE R%d %.9f", ed::sRob, a);
	}
	void Rotate(float a) {
		//	a*=0.0174532925;
		W("ROTATE R%d %.9f", ed::sRob, a);
	}
	void Pause() {
		W("PAUSE");
	}
	void Claw() {
		W("GARRA R%d", ed::sRob);
	}
	void CameraSel(int cam) {
		W("CAM %d", cam);
	}
	void RealWait(float t) {
		W("WAITreal %.9f", t);
	}
	
	void Wait(float t) {
		W("WAIT %.9f", t);
	}

	void GameSpeed(float v) {
		v = (v > 10) ? 10 : (v < 0) ? 0 : v;
		W("SPEED %.2f", v);
	}
	void Restart() {
		W("RESTART");
		Sleep(1000);
	}
	
	//FeedbackFunctions
	float GetTime() {
		R("getTIME");
		return atof(ed::chBuf);
	}
	int GetScore() {
		R("getScore");
		return atoi(ed::chBuf);
	}
	
	int GetCameraCount() {
		R("getCameraCount");
		return atoi(ed::chBuf);
	}
	
	int TestSensor(int i) { //-1 sensor inexistente , 0 sinal baixo , 1 sinal alto
		R("testSensor R%d %d", ed::sRob, i);
		return atoi(ed::chBuf);
	}

	
	//interromper todos comandos
	//quebrar fila
	//executar imediatamente


#undef ed
#undef W(X)
#undef R(X)
#pragma endregion

}
