using System;
using System.Drawing;

namespace LogoGen.Results
{
    public class BatchResult
    {
        public BatchResult(Bitmap image)
        {
            Image = image;
        }

        public BatchResult(Exception exception)
        {
            Exception = exception;
        }

        public Bitmap Image { get; }
        public Exception Exception { get; }
        public bool Succeeded => Exception == null;
    }
}