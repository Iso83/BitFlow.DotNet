#include "bitflow_native.h"

#include <BitFlow/core/expression/ExprPrinter.h>
#include <BitFlow/core/expression/ExprStore.h>

#include <BitFlow/core/rules/RulePipeline.h>

#include <BitFlow/io/ExprLatex.h>
#include <BitFlow/io/ExprParser.h>

#include <cstring>
#include <sstream>
#include <string>
#include <vector>

using namespace BitFlow::Core;
using namespace BitFlow::Core::Expression;
using namespace BitFlow::Core::Rules;
using namespace BitFlow::IO;

struct BF_Context_Internal {
	ExprStore store;
	ExprNameMap names;

	std::vector<std::string> trace;

	std::string lastError;
};

static char *CopyString(const std::string &text) {

	auto *result = new char[text.size() + 1];

	std::memcpy(
		result,
		text.c_str(),
		text.size() + 1
	);

	return result;
}

extern "C" {

	BF_Context BF_CreateContext() {

		try {
			return new BF_Context_Internal();
		}
		catch (...) {
			return nullptr;
		}
	}

	void BF_DestroyContext(BF_Context context) {
		delete static_cast<BF_Context_Internal *>(context);
	}

	int BF_Parse(
		BF_Context context,
		const char *expression,
		BF_ExprId *outExprId
	) {

		if (!context || !expression || !outExprId)
			return -1;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		try {

			auto result = Parse(
				&ctx->store,
				expression
			);

			ctx->names = result.names;

			*outExprId = result.root.id.value();

			return 0;
		}
		catch (const std::exception &ex) {

			ctx->lastError = ex.what();
			return -1;
		}
	}

	int BF_Rewrite(
		BF_Context context,
		BF_ExprId exprId,
		BF_ExprId *outExprId
	) {

		if (!context || !outExprId)
			return -1;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		try {

			ctx->trace.clear();

			RuleEngine engine(BuildExplore());
			engine.SetDebugCallback(
				[&](auto before, auto after, auto key) {

					auto beforeText = ToString(
						&ctx->store,
						before,
						ctx->names
					);

					auto afterText = ToString(
						&ctx->store,
						after,
						ctx->names
					);

					std::ostringstream ss;

					ss << "{";
					ss << "\"rule\":\"" << key.value << "\",";
					ss << "\"before\":\"" << beforeText << "\",";
					ss << "\"after\":\"" << afterText << "\"";
					ss << "}";

					ctx->trace.push_back(ss.str());
				}
			);

			auto rewritten = engine.Rewrite(
				&ctx->store,
				Ids::ExprId(exprId));

			*outExprId = rewritten.value();

			return 0;
		}
		catch (const std::exception &ex) {

			ctx->lastError = ex.what();
			return -1;
		}
	}

	const char *BF_ToString(
		BF_Context context,
		BF_ExprId exprId
	) {

		if (!context)
			return nullptr;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		try {

			auto text = ToString(
				&ctx->store,
				Ids::ExprId(exprId),
				ctx->names
			);

			return CopyString(text);
		}
		catch (const std::exception &ex) {

			ctx->lastError = ex.what();
			return nullptr;
		}
	}

	const char *BF_ToLatex(
		BF_Context context,
		BF_ExprId exprId
	) {

		if (!context)
			return nullptr;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		try {
			auto text = ToLatex(
				&ctx->store,
				Ids::ExprId(exprId),
				ctx->names
			);

			return CopyString(text);
		}
		catch (const std::exception &ex) {

			ctx->lastError = ex.what();
			return nullptr;
		}
	}

	const char *BF_GetTraceJson(
		BF_Context context
	) {

		if (!context)
			return nullptr;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		try {

			std::ostringstream ss;

			ss << "[";

			for (size_t i = 0; i < ctx->trace.size(); ++i) {

				if (i > 0)
					ss << ",";

				ss << ctx->trace[i];
			}

			ss << "]";

			return CopyString(ss.str());
		}
		catch (const std::exception &ex) {

			ctx->lastError = ex.what();
			return nullptr;
		}
	}

	void BF_FreeString(
		const char *value
	) {

		delete[] value;
	}

	const char *BF_GetLastError(
		BF_Context context
	) {

		if (!context)
			return nullptr;

		auto *ctx = static_cast<BF_Context_Internal *>(context);

		return ctx->lastError.c_str();
	}
}