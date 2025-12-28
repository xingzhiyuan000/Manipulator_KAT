using OpenTK;
using System;
using System.Numerics;
using System.Windows.Forms;

namespace OpenTK_Winform_Robot
{
    class Tools
    {
        public static void Decompose(
        Matrix4x4 matrix,
        out System.Numerics.Vector3 position,
        out System.Numerics.Vector3 eulerAngle,
        out System.Numerics.Vector3 scale)
        {
            bool isDecompose = Matrix4x4.Decompose(matrix, out System.Numerics.Vector3 scaleVector, out System.Numerics.Quaternion rotationQuaternion, out System.Numerics.Vector3 translation);

            if (!isDecompose) 
            {
                MessageBox.Show("Deconstruction failed");
            }
            position = translation;
            scale = scaleVector;

            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotationQuaternion);
            eulerAngle = ExtractEulerAnglesXYZ(rotationMatrix);

            eulerAngle = System.Numerics.Vector3.Multiply(eulerAngle, 180.0f / (float)Math.PI);
        }


        private static System.Numerics.Vector3 ExtractEulerAnglesXYZ(Matrix4x4 rotationMatrix)
        {
            float sy = (float)Math.Sqrt(rotationMatrix.M11 * rotationMatrix.M11 + rotationMatrix.M21 * rotationMatrix.M21);

            bool singular = sy < 1e-6f;          

            float x, y, z;
            if (!singular)
            {
                x = (float)Math.Atan2(rotationMatrix.M32, rotationMatrix.M33);
                y = (float)Math.Atan2(-rotationMatrix.M31, sy);
                z = (float)Math.Atan2(rotationMatrix.M21, rotationMatrix.M11);
            }
            else
            {
                x = (float)Math.Atan2(-rotationMatrix.M23, rotationMatrix.M22);
                y = (float)Math.Atan2(-rotationMatrix.M31, sy);
                z = 0;
            }

            return new System.Numerics.Vector3(x, y, z);
        }

        public static void Decompose(
        Matrix4 matrix,
        out OpenTK.Vector3 position,
        out OpenTK.Vector3 eulerAngle)
        {
            Matrix3 RX_90 = new Matrix3(
                            1, 0, 0,
                            0, 0, -1,
                            0, 1, 0
                            );

            position.X = matrix.M14;
            position.Y = matrix.M34;
            position.Z = -matrix.M24;

            Matrix3 subMatrix = new Matrix3(
                                matrix.M11, matrix.M12, matrix.M13,   
                                matrix.M21, matrix.M22, matrix.M23,   
                                matrix.M31, matrix.M32, matrix.M33    
                                );

           

            subMatrix = RX_90 * subMatrix;


            eulerAngle.Z= (float)(Math.Atan2(-subMatrix.M31, Math.Sqrt(Math.Pow(subMatrix.M11, 2) + Math.Pow(subMatrix.M21, 2))) * 180.0 / Math.PI);
            eulerAngle.X= (float)(Math.Atan2(subMatrix.M32 / Math.Cos(eulerAngle.Z), subMatrix.M33 / Math.Cos(eulerAngle.Z)) * 180 /Math.PI);
            eulerAngle.Y = (float)(Math.Atan2(subMatrix.M21 / Math.Cos(eulerAngle.Z), subMatrix.M11 / Math.Cos(eulerAngle.Z)) * 180 / Math.PI);

           
        }
    }
}
