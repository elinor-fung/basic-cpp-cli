#include "MixedLibrary.h"

#include <iostream>

extern "C" __declspec(dllexport) int __stdcall NativeEntryPoint(int i)
{
	std::cout << "Hello from NativeEntryPoint" << std::endl;
	return i + 1;
}