using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Xvue.Framework.Views.WPF.Controls
{
    public class Trackball
    {
        private Point _previousPosition2D;
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);
        private Quaternion _orientation = new Quaternion();
        private Transform3DGroup _transform;
        private ScaleTransform3D _scale = new ScaleTransform3D();
        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();


        public Trackball()
        {
            _transform = new Transform3DGroup();
            _transform.Children.Add(_scale);
            _transform.Children.Add(new RotateTransform3D(_rotation));
        }

        public void StartDrag(Point startPoint, double width, double height)
        {
            _previousPosition2D = startPoint;
            _previousPosition3D = ProjectToTrackball(width, height, _previousPosition2D);
        }

        public Transform3D Track(Point currentPosition, double width, double height)
        {
            Vector3D currentPosition3D = ProjectToTrackball(width, height, currentPosition);

            Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
            if (axis.Length < 100*double.Epsilon) return _transform;

            double angle = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D);
            
            Quaternion delta = new Quaternion(axis, -angle);

            _orientation = new Quaternion(_rotation.Axis, _rotation.Angle);
            // Compose the delta with the previous orientation
            _orientation *= delta;

            // Write the new orientation back to the Rotation3D
            _rotation.Axis = _orientation.Axis;
            _rotation.Angle = _orientation.Angle;

            _previousPosition3D = currentPosition3D;
            _previousPosition2D = currentPosition;
            return _transform;
        }

        public static Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);   // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y / (height / 2);
            x = Math.Min(x, 2.0) - 1;           // Translate 0,0 to the center
            y = 1 - Math.Min(y, 2.0);           // Flip so +Y is up instead of down

            double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        const double ZoomIncrement = 0.05;
        public Transform3D Zoom(int scaleDirection)
        {
            if (scaleDirection != 0)
            {
                double delta = (scaleDirection > 0) ? (1.0 - ZoomIncrement) : (1.0 + ZoomIncrement);
                _scale.ScaleX *= delta;
                _scale.ScaleY *= delta;
                _scale.ScaleZ *= delta;
            }
            return _transform;
        }

        public Transform3D Zoom(double distanceNewValue, double distanceOldValue)
        {
            double scaleDelta;
            try
            {
                scaleDelta = distanceOldValue / distanceNewValue;
            }
            catch(DivideByZeroException)
            {
                scaleDelta = 1;
            }

            _scale.ScaleX *= scaleDelta;
            _scale.ScaleY *= scaleDelta;
            _scale.ScaleZ *= scaleDelta;

            return _transform;
        }

        public Transform3D ResetTrackball()
        {
            _scale = new ScaleTransform3D();
            _rotation = new AxisAngleRotation3D();
            _orientation = new Quaternion();
            _transform.Children.Clear();
            _transform.Children.Add(_scale);
            _transform.Children.Add(new RotateTransform3D(_rotation));

            return _transform;
        }

        public void UpdateTransform(Transform3D transform)
        {
            Vector3D scaleVector;
            Quaternion roatationQuaternion;
            if (Decompose(transform.Value, out scaleVector, out roatationQuaternion))
            {
                _rotation.Axis = roatationQuaternion.Axis;
                _rotation.Angle = roatationQuaternion.Angle;
                _scale.ScaleX = scaleVector.X;
                _scale.ScaleY = scaleVector.Y;
                _scale.ScaleZ = scaleVector.Z;
            }
        }
        /// <summary>
        /// Decomposes the specified matrix. Extracts the scale and rotation components from a 3D scale/rotate (SR) matrix.
        /// </summary>
        /// <param name="matrix">The matrix.  Assumes a scale, rotate transform matrix</param>
        /// <param name="scale">The scale.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns><c>true</c> if decomposition is successful, <c>false</c> otherwise.</returns>
        private static bool Decompose(Matrix3D matrix, out Vector3D scale, out Quaternion rotation)
        {
            bool result = true;
            //fixed (float* pfScales = &scale.X)            
            float det;
            scale = new Vector3D();
            Vector3D[] vectorBasis = new Vector3D[3];

            Matrix3D matTemp =  Matrix3D.Identity;
            Vector3D[] canonicalBasis = new Vector3D[3];

            canonicalBasis[0] = new Vector3D(1.0f, 0.0f, 0.0f);
            canonicalBasis[1] = new Vector3D(0.0f, 1.0f, 0.0f);
            canonicalBasis[2] = new Vector3D(0.0f, 0.0f, 1.0f);

            //pVectorBasis[0] = (Vector3D*)&matTemp.M11;
            //pVectorBasis[1] = (Vector3D*)&matTemp.M21;
            //pVectorBasis[2] = (Vector3D*)&matTemp.M31;

            vectorBasis[0] = new Vector3D(matrix.M11, matrix.M12, matrix.M13);
            vectorBasis[1] = new Vector3D(matrix.M21, matrix.M22, matrix.M23);
            vectorBasis[2] = new Vector3D(matrix.M31, matrix.M32, matrix.M33);

            scale.X = vectorBasis[0].Length;
            scale.Y = vectorBasis[1].Length;
            scale.Z = vectorBasis[2].Length;

            uint a, b, c;
            #region Ranking
            float[] fScales = new float[3];
            fScales[0] = (float)scale.X;
            fScales[1] = (float)scale.Y;
            fScales[2] = (float)scale.Z;
            float x = fScales[0], y = fScales[1], z = fScales[2];
            if (x < y)
            {
                if (y < z)
                {
                    a = 2;
                    b = 1;
                    c = 0;
                }
                else
                {
                    a = 1;

                    if (x < z)
                    {
                        b = 2;
                        c = 0;
                    }
                    else
                    {
                        b = 0;
                        c = 2;
                    }
                }
            }
            else
            {
                if (x < z)
                {
                    a = 2;
                    b = 0;
                    c = 1;
                }
                else
                {
                    a = 0;

                    if (y < z)
                    {
                        b = 2;
                        c = 1;
                    }
                    else
                    {
                        b = 1;
                        c = 2;
                    }
                }
            }
            #endregion

            if (fScales[a] < float.Epsilon)
            {
                vectorBasis[a] = canonicalBasis[a];
            }

            vectorBasis[a].Normalize();

            if (fScales[b] < float.Epsilon)
            {
                uint cc;
                float fAbsX, fAbsY, fAbsZ;

                fAbsX = (float)Math.Abs(vectorBasis[a].X);
                fAbsY = (float)Math.Abs(vectorBasis[a].Y);
                fAbsZ = (float)Math.Abs(vectorBasis[a].Z);

                #region Ranking
                if (fAbsX < fAbsY)
                {
                    if (fAbsY < fAbsZ)
                    {
                        cc = 0;
                    }
                    else
                    {
                        if (fAbsX < fAbsZ)
                        {
                            cc = 0;
                        }
                        else
                        {
                            cc = 2;
                        }
                    }
                }
                else
                {
                    if (fAbsX < fAbsZ)
                    {
                        cc = 1;
                    }
                    else
                    {
                        if (fAbsY < fAbsZ)
                        {
                            cc = 1;
                        }
                        else
                        {
                            cc = 2;
                        }
                    }
                }
                #endregion

                vectorBasis[b] = Vector3D.CrossProduct(vectorBasis[a], canonicalBasis[cc]);
            }

            vectorBasis[b].Normalize();

            if (fScales[c] < float.Epsilon)
            {
                vectorBasis[c] = Vector3D.CrossProduct(vectorBasis[a], vectorBasis[b]);
            }

            vectorBasis[c].Normalize();

            det = (float)matTemp.Determinant;

            // use Kramer's rule to check for handedness of coordinate system
            if (det < 0.0f)
            {
                // switch coordinate system by negating the scale and inverting the basis vector on the x-axis
                fScales[a] = -fScales[a];
                vectorBasis[a] = -(vectorBasis[a]);

                det = -det;
            }

            det -= 1.0f;
            det *= det;

            if ((float.Epsilon < det))
            {
                // Non-SRT matrix encountered
                rotation = Quaternion.Identity;
                result = false;
            }
            else
            {
                // generate the quaternion from the matrix
                matTemp.M11 = vectorBasis[0].X; matTemp.M12 = vectorBasis[0].Y; matTemp.M13 = vectorBasis[0].Z;
                matTemp.M21 = vectorBasis[1].X; matTemp.M22 = vectorBasis[1].Y; matTemp.M23 = vectorBasis[1].Z;
                matTemp.M31 = vectorBasis[2].X; matTemp.M32 = vectorBasis[2].Y; matTemp.M33 = vectorBasis[2].Z;
                rotation = CreateFromRotationMatrix(matTemp);
            }
            return result;
        }

        private static Quaternion CreateFromRotationMatrix(Matrix3D matrix)
        {
            float trace = (float)(matrix.M11 + matrix.M22 + matrix.M33);

            Quaternion q = new Quaternion();

            if (trace > 0.0f)
            {
                float s = (float)Math.Sqrt(trace + 1.0f);
                q.W = s * 0.5f;
                s = 0.5f / s;
                q.X = (matrix.M23 - matrix.M32) * s;
                q.Y = (matrix.M31 - matrix.M13) * s;
                q.Z = (matrix.M12 - matrix.M21) * s;
            }
            else
            {
                if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
                {
                    float s = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                    float invS = 0.5f / s;
                    q.X = 0.5f * s;
                    q.Y = (matrix.M12 + matrix.M21) * invS;
                    q.Z = (matrix.M13 + matrix.M31) * invS;
                    q.W = (matrix.M23 - matrix.M32) * invS;
                }
                else if (matrix.M22 > matrix.M33)
                {
                    float s = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                    float invS = 0.5f / s;
                    q.X = (matrix.M21 + matrix.M12) * invS;
                    q.Y = 0.5f * s;
                    q.Z = (matrix.M32 + matrix.M23) * invS;
                    q.W = (matrix.M31 - matrix.M13) * invS;
                }
                else
                {
                    float s = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                    float invS = 0.5f / s;
                    q.X = (matrix.M31 + matrix.M13) * invS;
                    q.Y = (matrix.M32 + matrix.M23) * invS;
                    q.Z = 0.5f * s;
                    q.W = (matrix.M12 - matrix.M21) * invS;
                }
            }

            return q;
        }
    }

}
