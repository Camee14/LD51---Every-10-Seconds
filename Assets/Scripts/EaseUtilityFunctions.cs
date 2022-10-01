using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class EaseUtilityFunctions 
{
    const float c4  = (2f * math.PI) / 3f;
    
    public static float easeOutElastic(float x){
        return x == 0f ? 0f : x == 1f ? 1f : math.pow(2f, -10f * x) * math.sin((x * 10f - 0.75f) * c4) + 1f;
    }
    public static float easeInElastic(float x) {
        return x == 0f ? 0f : x == 1f ? 1f : -math.pow(2f, 10f * x - 10f) * math.sin((x * 10f - 10.75f) * c4);
    }
    public static float easeInQuart(float x){
        return x * x * x * x;
    }
    public static float easeOutQuart(float x){
        return 1 - math.pow(1 - x, 4);
    }
    public static float easeInQuad(float x){
        return x * x;
    }
    public static float easeOutQuad(float x){
        return 1 - (1 - x) * (1 - x);
    }
    public static float easeInOutQuad(float x){
        return x < 0.5 ? 2 * x * x : 1 - math.pow(-2 * x + 2, 2) / 2;
    }
}
