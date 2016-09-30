namespace cstsd.TestAssembly.CSharp
{
    public class CrazyAmountOfNestedClasses
    {
        public class C1
        {
            public class C2
            {
                public class C3
                {
                    public class C4
                    {
                        public class C5
                        {
                            public class WAT
                            {
                                public static void TakesANestedParam(WAT wat)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
