#include <windows.h>
#include <tuple>

#import "server.tlb"
using namespace ServerLib;

int main()
{
	std::ignore = CoInitialize(nullptr);
	IServerPtr server;
	if (SUCCEEDED(server.CreateInstance(__uuidof(Server))))
	{
		auto pi = server->ComputePi();
		printf("pi:%f", pi);
	}
	CoUninitialize();
}
