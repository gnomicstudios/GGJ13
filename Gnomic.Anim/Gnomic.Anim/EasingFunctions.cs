using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gnomic.Anim
{

//// TODO - from the Vector2[], create a list of KeySpline objects.

//// FROM : http://blog.greweb.fr/2012/02/bezier-curve-based-easing-functions-from-concept-to-implementation/
///**
//* KeySpline - use bezier curve for transition easing function
//* is inspired from Firefox's nsSMILKeySpline.cpp
//* Usage:
//* var spline = new KeySpline(0.25, 0.1, 0.25, 1.0)
//* spline.get(x) => returns the easing value | x must be in [0, 1] range
//*/
//function KeySpline (mX1, mY1, mX2, mY2) {

//  this.get = function(aX) {
//    if (mX1 == mY1 && mX2 == mY2) return aX; // linear
//    return CalcBezier(GetTForX(aX), mY1, mY2);
//  }

//  function A(aA1, aA2) { return 1.0 - 3.0 * aA2 + 3.0 * aA1; }
//  function B(aA1, aA2) { return 3.0 * aA2 - 6.0 * aA1; }
//  function C(aA1)      { return 3.0 * aA1; }

//  // Returns x(t) given t, x1, and x2, or y(t) given t, y1, and y2.
//  function CalcBezier(aT, aA1, aA2) {
//    return ((A(aA1, aA2)*aT + B(aA1, aA2))*aT + C(aA1))*aT;
//  }

//  // Returns dx/dt given t, x1, and x2, or dy/dt given t, y1, and y2.
//  function GetSlope(aT, aA1, aA2) {
//    return 3.0 * A(aA1, aA2)*aT*aT + 2.0 * B(aA1, aA2) * aT + C(aA1);
//  }

//  function GetTForX(aX) {
//    // Newton raphson iteration
//    var aGuessT = aX;
//    for (var i = 0; i < 4; ++i) {
//      var currentSlope = GetSlope(aGuessT, mX1, mX2);
//      if (currentSlope == 0.0) return aGuessT;
//      var currentX = CalcBezier(aGuessT, mX1, mX2) - aX;
//      aGuessT -= currentX / currentSlope;
//    }
//    return aGuessT;
//  }
//}
	public static class Easing
	{
		public static float QuadraticEaseIn(float t, float strength)
		{
			float t2 = 1.0f - strength + t * strength;
			return t * t2;
		}
		
		public static float QuadraticEaseOut(float t, float strength) 
		{
			float t2 = 1.0f - strength + t * strength;
			return 2.0f * t - t * t2;
		}

		//public static Vector2 CubicBezierCurve(Vector2 start, Vector2 curve1, Vector2 curve2, Vector2 end, float t)
		//{
		//    float tPow2 = t * t;
		//    float wayToGo = 1.0f - t;
		//    float wayToGoPow2 = wayToGo * wayToGo;

		//    return wayToGo * wayToGoPow2 * start
		//           + 3.0f * t * wayToGoPow2 * curve1
		//           + 3.0f * tPow2 * wayToGo * curve2
		//           + t * tPow2 * end;
		//}

		//float QuadraticEaseIn(float a, float b, float t, float strength) 
		//{
		//    float c = b - a;
		//    float t2 = 1.0f - strength + t * strength;
		//    return a + c * t * t2;
		//}
		
		//float QuadraticEaseOut(float a, float b, float t, float strength) 
		//{
		//    float c = b - a;
		//    float t2 = 1.0f - strength + t * strength;
		//    return a - (c * t * t2) + (2 * c * t);
		//}

		//// quadratic easing in/out - acceleration until halfway, then deceleration
		//Math.easeInOutQuad = function (t, b, c, d) {
		//    if (t < d/2) return 2*c*t*t/(d*d) + b;
		//    var ts = t - d/2;
		//    return -2*c*ts*ts/(d*d) + 2*c*ts/d + c/2 + b;
	}
}
