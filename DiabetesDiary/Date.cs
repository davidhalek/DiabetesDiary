using System;

/// <summary>
/// Date object 
/// </summary>
namespace DiabetesDiary
{
    internal class Date
    {
        private int year;
        private int v1;
        private int v2;

        public Date(int year, int v1, int v2)
        {
            this.year = year;
            this.v1 = v1;
            this.v2 = v2;
        }

        internal object AddDays(int v)
        {
            throw new NotImplementedException();
        }
    }
}