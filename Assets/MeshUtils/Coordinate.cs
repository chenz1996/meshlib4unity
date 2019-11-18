using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Coordinate {
    //局部坐标变换到世界坐标系
    public static Vector3 localToWorld(this Vector3 position, Transform transform) {
        Vector4 posV4 = new Vector4(position.x, position.y, position.z, 1f);
        posV4 = transform.localToWorldMatrix * posV4;
        return new Vector3(posV4.x, posV4.y, posV4.z);
    }
}
