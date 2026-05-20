#pragma once

#include <stdint.h>

#ifdef _WIN32
#define BF_API __declspec(dllexport)
#else
#define BF_API __attribute__((visibility("default")))
#endif

extern "C" {

    typedef void *BF_Context;
    typedef uint32_t BF_ExprId;

    BF_API const char *BitFlow_GetGitHash();

    BF_API BF_Context BF_CreateContext();
    BF_API void BF_DestroyContext(BF_Context context);

    BF_API int BF_Parse(
        BF_Context context,
        const char *expression,
        BF_ExprId *outExprId
    );

    BF_API int BF_Rewrite(
        BF_Context context,
        BF_ExprId exprId,
        BF_ExprId *outExprId
    );

    BF_API const char *BF_ToString(
        BF_Context context,
        BF_ExprId exprId
    );

    BF_API const char *BF_ToLatex(
        BF_Context context,
        BF_ExprId exprId
    );

    BF_API const char *BF_GetTraceJson(
        BF_Context context
    );

    BF_API void BF_FreeString(
        const char *value
    );

    BF_API const char *BF_GetLastError(
        BF_Context context
    );

}