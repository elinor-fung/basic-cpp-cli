#pragma once

using namespace System;

namespace MixedLibrary {
	public ref class Test
	{
	public:
		static int Run()
		{
			Console::WriteLine("Hello from MixedLibrary managed class");
			return 42;
		}
	};
}
