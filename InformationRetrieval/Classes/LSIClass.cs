using System;

//using Bluebit.MatrixLibrary; 
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Numerics;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace InformationRetrieval.Classes
{
    public class LSIClass
    {
        static double[,] A = new double[,] { };
        static double[] q = new double[] { };
        string folder = @"/Users/User/Documents/itis/Basic_Of_Inf_retrieval/Inf_retrieval/InformationRetrieval/InformationRetrieval/";
        string _mess;
        List<int> ids;
        Matrix<double> mass;

        public LSIClass(double[,] B, double[] Q, string mess, List<int> n)
        {
            A = B;
            q = Q;
            _mess = mess;
            ids = n;
        }


        public void LatentSemanticIndexing()
        {


            mass = DenseMatrix.OfArray(A);
            var svd = mass.Svd(true);
            Matrix<double> U = svd.U;
            Matrix<double> S = svd.W;
            Matrix<double> V = svd.VT;

            Matrix<double> Uk = RemoveColumns(U);
            Matrix<double> Sk = RemoveColumnsAndRows(S);
            Matrix<double> Vk = RemoveRows(V);

            Console.WriteLine("Uk = " + Uk);
            Console.WriteLine("Sk = " + Sk);
            Console.WriteLine("Vk = " + Vk);






            //q = qT Uk Sk-1 
            Matrix<double> queryMatrix = new DenseMatrix(q.Length, 1, q);

            //queryMatrix = queryMatrix.Transpose() * (Uk * Sk.Inverse());
            queryMatrix = queryMatrix.Transpose() * (Uk * Sk.Inverse());

            Vk = Sk.Inverse() * Vk;
            Dictionary<int, double> dic = new Dictionary<int, double>();
            //sim(q, d) = sim(qT Uk Sk-1, dT Uk Sk-1) using cosine similarities 
            for (int i = 0; i < Vk.ColumnCount; i++)
            {
                Vector docVector = (MathNet.Numerics.LinearAlgebra.Double.Vector)Vk.Column(i);
                Vector queryVector = (MathNet.Numerics.LinearAlgebra.Double.Vector)queryMatrix.Row(0);
                //double sim0 = Distance.Cosine(docVector.ToArray(), queryVector.ToArray());
                double sim = CosSim(docVector, queryVector);
                dic.Add(i, sim);
                //double sim = docVector.DotProduct(queryVector) / (docVector.Count * queryVector.Count);
                Console.WriteLine("Doc" + i + " : " + sim);
            }
           string output = SortedByLSA(dic);
            File.WriteAllText(folder + "FilesXML/HomeWork6.txt", output);

        }

        public double CosSim(Vector a, Vector b)
        {
            double output = a.DotProduct(b);
            double doc = 0.0;
            double query = 0.0;
            for (int i = 0; i < a.Count; i++)
            {
                doc += a[i] * a[i];
                query += b[i] * b[i];
            }

            doc = Math.Sqrt(doc);
            query = Math.Sqrt(query);
            output = output / (doc + query);
            return output;
        }

        public Matrix<double> RemoveColumns(Matrix<double> A)
        {
            Matrix<double> B = new DenseMatrix(A.ColumnCount, A.RowCount);
            B = A.Clone();
            int count = A.ColumnCount - 1;
            while (count > 1)
            {
                B = B.RemoveColumn(count);
                count = B.ColumnCount - 1;
            }
            return B;
        }
        public Matrix<double> RemoveRows(Matrix<double> A)
        {
            Matrix<double> B = new DenseMatrix(A.ColumnCount, A.RowCount);
            B = A.Clone();
            int count = A.RowCount - 1;
            while (count > 1)
            {
                B = B.RemoveRow(count);
                count = B.RowCount - 1;
            }
            return B;
        }
        public Matrix<double> RemoveColumnsAndRows(Matrix<double> A)
        {
            Matrix<double> B = RemoveColumns(A);
            B = RemoveRows(B);
            return B;

        }

        public string SortedByLSA(Dictionary<int, double> sim)
        {
            string output = null;
            Dictionary<int, double> filterSim = new Dictionary<int, double>();
            for (int i = 0; i < ids.Count(); i++){
                if (sim.ContainsKey(ids[i])){
                    filterSim.Add(ids[i], sim[i]);
                }
            }
            filterSim = filterSim.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            output += "LSA for ( " + _mess + " ): \n";
            output += "A : \n";
            for (int i = 0; i < mass.RowCount; i++)
            {
                for (int j = 0; j < mass.ColumnCount; j++)
                {
                    if (mass[i, j].Equals(0))
                        output += mass[i, j] + "           ";
                    else
                    output += Math.Round(mass[i, j], 3) + "       ";
                }
                output += "\n";
            }

            foreach (int key in filterSim.Keys)
            {
                output += "\n Id: " + key + "     sim:" + filterSim[key];
            }

            return output;
        }


    }
}



