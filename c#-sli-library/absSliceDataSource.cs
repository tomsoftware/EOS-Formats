using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThermoBox.slicedata
{

    public class ty_Matrix3x2
    {
        public double m00 = 1; public double m10 = 0; public double m20 = 0;
        public double m01 = 0; public double m11 = 1; public double m21 = 0;


        //-------------------------------------------------------//
        public static ty_Matrix3x2 MatrixMult(ty_Matrix3x2 A, ty_Matrix3x2 B)
        {
            ty_Matrix3x2 C = new ty_Matrix3x2();

            C.m00 = A.m00 * B.m00 + A.m10 * B.m01 + A.m20 * 0;
            C.m10 = A.m00 * B.m10 + A.m10 * B.m11 + A.m20 * 0;
            C.m20 = A.m00 * B.m20 + A.m10 * B.m21 + A.m20 * 1;

            C.m01 = A.m01 * B.m00 + A.m11 * B.m01 + A.m21 * 0;
            C.m11 = A.m01 * B.m10 + A.m11 * B.m11 + A.m21 * 0;
            C.m21 = A.m01 * B.m20 + A.m11 * B.m21 + A.m21 * 1;

            return C;
        }
    }

    public interface absSliceDataSource
    {
        //------------------------------------------//
        void setFileName(System.String filename);

        //------------------------------------------//
        System.String getFileName();

        //------------------------------------------//
        int getLayerCount();

        //------------------------------------------//
        float getLayerThickness();

        //------------------
        float getLayerUpPosition(int LayerIndex);


        //------------------------------------------//
        int getObjectCount();

        //-------------------------------------------//
        System.String getObjectName(int ObjectIndex);
        System.String getObjectInfo(int ObjectIndex);

        bool getObjectEnabled(int ObjectIndex);
        void setObjectEnabled(int ObjectIndex, bool enable);
               



        //------------------------------------------//
        clSliceData getSliceData(int ObjectIndex, int LayerIndex, float JobLayerThickness_mm);
        ty_Matrix3x2 getSliceTransformMatrix(int ObjectIndex, int LayerIndex);

    }
}
