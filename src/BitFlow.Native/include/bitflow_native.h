#pragma once

#ifdef _WIN32
#define BF_API __declspec(dllexport)
#else
#define BF_API
#endif

extern "C" {

	BF_API int BF_Add(int a, int b);

}