#ifndef AVL_LIGHTING_LIB_INCLUDED
#define AVL_LIGHTING_LIB_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "AVLCommon.hlsl"

float SoftenIntegralEdge(float integral, float rayLength, float lightRadius, float lightScattering) // ToDo calculate on CPU
{
    const float edgeIntegral = rayLength / (lightRadius * lightRadius + lightScattering);
    integral -= edgeIntegral;
        
    const float scattering = ClampAway(lightScattering, 0.0, 0.0001);
            
    float a0max = 1.0 / scattering;
    float amax = 1.0 / scattering - 1.0 / (lightRadius * lightRadius + scattering);
            
    integral *= (a0max / amax);

    return integral;
}

/*
 * Wolfram Integral Function: Integrate[Divide[1,Power[\(40)Subscript[y,0]+Subscript[y,1]*t\(41),2]+S],t]
 * Integrates from 0 to L
 */
float SolveAreaHardEdgeLightIntegral1D(float y0, float y1, float L, float S)
{
    y1 = ClampAway(y1, 0.0, 0.01);
    S = ClampAway(S, 0.0, 0.001);
    
    const float a = rsqrt(abs(S));
    
    const float J = a / y1;
    return (atan((L * y1 + y0) * a) - atan(y0 * a)) * J;
}

/*
 * Wolfram Integral Function: Integrate[Divide[1,Power[\(40)Subscript[y,0]+Subscript[y,1]*t\(41),2]],t]
 * Integrates from 0 to L
 */
float SolveAreaHardEdgeLightIntegralNoScattering1D(float y0, float y1, float L)
{
    return -1.0 / (y1 * (L * y1 + y0)) + 1.0 / (y1 * y0);
}

/*
 * Wolfram Integral Function: Divide[1,Power[\(40)x_0+t\(41),2]+Power[y,2]+U]dt
 * Integrates from 0 to A
 */
float SolvePointLightIntegral1D(float x0, float y0, float A, float lambda)
{
    float J = rsqrt(lambda + y0*y0);
    float b = x0*J;
    float a = b + A*J;
    return (FastATan(a) - FastATan(b)) * J;
}

/**
 * \brief Solution of a definite integral for the spotlight's soft edge.
 * Mathematica function being integrated: Integrate[1/((Subscript[x, 0] + t)^2 + Subscript[y, 0]^2 + S)*(1 - (((Subscript[u, 0] + u*t)^2 + Subscript[w, 0]^2)/((Subscript[x, 0] + t)^2 + Subscript[y, 0]^2 + S) - Subscript[T, 1])/Subscript[T, 0]), t]
 * \param x0 1D-space ray origin x
 * \param y0 1D-space ray origin y
 * \param u0 2D-space ray origin x
 * \param w0 2D-space ray origin y
 * \param u 2D-space ray direction x
 * \param t Ray Length
 * \param s Scattering
 * \param alpha Sine of (Primary Angle / 2)
 * \param beta Sine of (Secondary Angle / 2)
 * \return 
 */
float SolveSpotlightEdgeIntegralV2(float x0, float y0, float u0, float w0, float u, float t, float s, float alpha, float beta)
{
    const float t0 = alpha * alpha - beta * beta;
    const float t1 = beta * beta;

    const float us = u*u;
    const float u0s = u0*u0;
    const float y0s = y0*y0;
    const float x0s = x0*x0;
    const float x0c = x0s*x0;
    const float w0s = w0*w0;
    const float ts = t*t;

    const float term0 = rsqrt(s + y0s);
    const float term1 = pow(abs(s + y0s), 1.5);
    const float term2 = s*us + u0s + w0s - 2*u*u0*x0 + us*x0s + us*y0s - 2.0*t0*(s + y0s) - 2.0*t1*(s + y0s);
    const float term3 = s + x0s + y0s;
    const float term4 = s + y0s;
    const float term5 = t + x0;
    const float term6 = 2*u*u0;

    return (
        (atan(x0*term0) - atan(term5*term0)) * term2/term1 +
        (-(u0s*term5) - w0s*term5 + term6*(t*x0 + term3) + us*(-(t*x0s) - x0c + t*term4 - x0*term4)) /
        (term4*(ts + 2*t*x0 + term3)) +
        (u0s*x0 - term6*term3 + x0*(w0s + us*term3)) /
        (term4*term3))
        / (2.*t0);
}

float SolveTestSpotIntegral(float x0, float y0, float x, float u0, float v_0, float w0, float u, float v, float t, float S, float angle) // ToDo make FastATan ver
{
    // Avoid division by zero
    x0 = ClampAway(x0, 0.0, 0.00001);
    
    const float T = (sin(angle) * sin(angle)); // ToDo

    const float x_0s = x0*x0;
    const float ts = t*t;
    const float y_0s = y0*y0;
    const float us = u*u;
    const float w_0s = w0*w0;

    float term0 = S + y_0s;
    float term1 = t + x0;
    float term2 = 2.0 * T;
    const float term3 = w_0s + pow(u0 - u * x0, 2.0f);
    const float term4 = rsqrt(term0);
    const float term5 = (S * (us - term2) + term3 - term2 * y_0s + us * y_0s) / pow(term0, 1.5f);

    // Avoid division by zero
    term0 = ClampAway(term0, 0.0, 0.00001);
    term1 = ClampAway(term1, 0.0, 0.00001);
    term2 = ClampAway(term2, 0.0, 0.00001);

    const float integral = -(atan(term1 * term4) * term5) + 
          (-(term3 / term0) + (pow(t*u + u0, 2.0f) + w_0s) /
          ( S + ts + 2.0f * t * x0 + x_0s + y_0s)) / term1;
    
    const float integral0 = -(atan(x0 * term4) * term5) + 
          (-(term3 / term0) + (pow(u0, 2.0f) + w_0s) /
          ( S + x_0s + y_0s)) / x0;
    
    return (integral - integral0) / term2;
}

/*
 * Wolfram Integral Function: Integrate[Divide[1,Power[x+t,2]+Power[y,2]+Power[U,2]*Power[t,2]],t]
 * Integrates from 0 to distance
 */
inline float SolvePointLightIntegralWithScattering1D(float x0, float y0, float distance, float scatteringFactor)
{
    const float d = distance;
    const float s = scatteringFactor*scatteringFactor;
    const float r = rsqrt(s*(x0*x0 + y0*y0)+y0*y0);
    return r * (FastATan((d * s + d + x0) * r) - FastATan(x0 * r));
}

/*
* Wolfram Integral Function: Divide[1,\(40)Power[\(40)x_0+x_1*t\(41),2]+Power[\(40)y_0+y_1*t\(41),2]+Power[z_0,2]+U\(41)]dt
* Integrates from 0 to A
*/
float SolvePointLightIntegral2D(float x0, float y0, float z0, float x1, float y1, float A, float lambda)
{
    float x0s = x0*x0;
    float y0s = y0*y0;
    float z0s = z0*z0;
    float x1s = x1*x1;
    float y1s = y1*y1;

    float denominator = rsqrt(x1s*(lambda+y0s+z0s)+y1s*(lambda+x0s+z0s)-2.0*x0*x1*y0*y1);

    float numerator0 = FastATan((y0*y1+x0*x1)*denominator);
    float numerator1 = FastATan((A*x1s + y1*(A*y1+y0) + x0*x1)*denominator);
        
    return (numerator1 - numerator0)*denominator;
}

/*
* Wolfram Integral Function: Divide[1,\(40)Power[\(40)x_0+x_1*t\(41),2]+Power[\(40)y_0+y_1*t\(41),2]+W\(41)*\(40)Divide[T_1,T_0-\(40)Divide[Power[\(40)y_0+y_1*t\(41),2],Power[\(40)x_0+x_1*t\(41),2]+Power[\(40)y_0+y_1*t\(41),2]+Power[z,2]]\(41)]\(41)] dt
* Integrates from 0 to A
*
* Integrate[Divide[1,\(40)Power[\(40)u+x*t\(41),2]+Power[\(40)v+y*t\(41),2]+S+Power[w,2]\(41)*\(40)Divide[Subscript[T,1],Subscript[T,0]-\(40)Divide[Power[\(40)v+y*t\(41),2],Power[\(40)u+x*t\(41),2]+Power[\(40)v+y*t\(41),2]+Power[w,2]]\(41)]\(41)],t]
*/
float SolveSpotlightEdgeIntegral(float x0, float y0, float z0, float x1, float y1, float cosAlpha, float cosBeta, float t, float lambda)
{
    const float ts = t*t;
    
    const float x0s = x0*x0;
    const float y0s = y0*y0;
    const float z0s = z0*z0;
    
    const float x1s = x1*x1;
    const float y1s = y1*y1;
    
    const float x1c = x1s*x1;
    const float y1c = y1s*y1;
    
    const float x1q = x1s*x1s;
    const float y1q = y1s*y1s;
    
    const float x0x1 = x0*x1; // yes
    const float x0x1d = 2.0*x0x1; // no
    const float y0y1 = y0*y1; // no
    const float x0x1y0y1 = x0x1*y0y1; // yes?
    const float x0x1y0y1d = 2.0*x0x1y0y1; // yes?
    const float x1sy1s = x1s*y1s; // no
    
    const float cosas = cosAlpha*cosAlpha; //Inner - Alpha no
    const float cosbs = cosBeta*cosBeta; //Outer - Beta no
    
    const float T0 = cosbs; // no
    const float T1 = T0 - cosas; // no
    
    const float W = z0s+lambda;
    
    const float H = x0x1+y0y1; // no
    const float E = x1s+y1s;
    const float A = ts*E + 2.0*t*H;
    const float B = W + x0s + y0s;
    const float C = x1*y1*(x1*y0-x0*y1);
    const float D = W - z0s;
    const float Es = E*E;
    const float F = x0s+y0s+z0s;
    const float G = 2.0*x0*x1c*y0y1;
    const float J = x0s+z0s;
    const float K = y0s+z0s;
    const float L = t*x1s+y1*(t*y1+y0)+x0x1; // no
    
    const float T1Es = T1*Es; // no
    
    float sqrt0 = rsqrt(x1s*K+y1s*J-x0x1y0y1d); // no
    float sqrt1 = rsqrt(x1s*(W+y0s)+y1s*(x0s+W)-x0x1y0y1d); // no
    
    const float V0 = atan(H*sqrt1); // no
    const float V1 = atan(L*sqrt1); // no
    const float V2 = (T0*D*Es-x1sy1s*(W-x0s+y0s)-y1q*(W+x0s)+x1q*y0s-G+x0x1d*y0*y1c)/D*sqrt1; // no
    
    float result = 0.0;
    result += C/D*log(F*(A+B) / (B*(A+F)));
    result += V2*(V1-V0);
    result += (x1sy1s*(-x0s+K)+y1q*J+x1q*-y0s+G-x0x1d*y0*y1c)*-sqrt0/D*(atan(H*sqrt0)-atan(L*sqrt0));
    result /= T1Es;
    
    return result;
}

// ====================
// ShaderGraph Wrappers
// ====================

void SolvePointLightIntegral1D_float(float x0, float y0, float A, float lambda, out float o_result)
{
    o_result = SolvePointLightIntegral1D(x0, y0, A, lambda);
}

#endif