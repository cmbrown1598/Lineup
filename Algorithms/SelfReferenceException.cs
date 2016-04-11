using System;

namespace Algorithms
{
    public class SelfReferenceException : Exception
    {
        public SelfReferenceException() : base("Detected a self reference. Aborting.")
        {
            
        }
    }
}