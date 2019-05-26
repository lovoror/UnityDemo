#if defined(ANDROID_ETC1)
	#define TEX2D_ALPHA( sampler, tex )			float4( tex2D( sampler, tex ).rgb, tex2D( sampler##_A, tex ).r )
#endif